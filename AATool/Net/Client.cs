using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using AATool.Settings;
using Newtonsoft.Json;

namespace AATool.Net
{
    public sealed class Client : Peer
    {
        public bool IsConnecting { get; private set; }

        private bool wasKickedByServer;

        private readonly Dictionary<string, string> recieved;
        private readonly Queue<Message> sendQueue;
        private readonly Socket socket;

        public Client() : base()
        {
            this.socket       = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.sendQueue    = new();
            this.recieved     = new();
            this.IsConnecting = true;
        }

        public static bool TryGet(out Client client)
        {
            client = Instance as Client;
            return client is not null;
        }

        public override bool Connected() => !this.IsConnecting && this.socket.Connected;

        public bool TryGetData(string key, out string data) => this.recieved.TryGetValue(key, out data);

        protected override void Start(IPAddress address, int port, Uuid id)
        {
            base.Start(address, port, id);
            if (id == Uuid.Empty)
            {
                string reason = Player.ValidateName(Config.Network.MinecraftName)
                    ? "Couldn't get UUID. Either Mojang's servers didn't respond or an account with the requested name doesn't exist."
                    : "Invalid Minecraft name.";
                this.Stop("Error starting client: " + reason);
                return;
            }

            WriteToConsole("Attempting connection...");
            UpdateControls("Connecting...", false, false);

            try
            {
                //attempt connection
                IPEndPoint endPoint = new (address, port);
                IAsyncResult ar = this.socket.BeginConnect(endPoint, this.ConnectCallback, this.socket);

                //set a timeout for attempt connection
                if (ar.AsyncWaitHandle.WaitOne(Protocol.TIMEOUT_MS, true) && this.socket.Connected)
                {
                    //start recieving messages from server 
                    this.socket.BeginReceive(Buffer, 0, Protocol.BUFFER_SIZE, SocketFlags.None, this.ReceiveCallback, null);
                }
                else
                {
                    //couldn't establish connection with server
                    this.IsConnecting = false;
                    this.socket.Close();
                    this.Stop("Connection Timeout: Couldn't reach server.");
                }
            }
            catch (Exception e)
            {
                this.Stop($"Error starting client: {e.Message}.");
            }
        }

        protected override void Stop(string reason)
        {
            this.IsConnecting = false;
            try
            {
                if (this.socket.Connected)
                {
                    if (!this.wasKickedByServer)
                    {
                        //tell server
                        this.SendToServer(Message.LogOut());
                    }

                    //close connection to server
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Close();
                }
            }
            catch { }
            base.Stop(reason);

            //re-enable controls
            UpdateControls("Connect", true, true);
            StateChangedFlag = true;
        }

        public void SendToServer(Message message)
        {
            if (!this.socket.Connected)
                return;

            //enqueue message and send immediately if only message in pipeline
            try
            {
                //compress and send string to server
                byte[] compressed = NetworkHelper.CompressString(message.ToString());
                this.socket.BeginSend(compressed, 0, compressed.Length, SocketFlags.None, this.SendCallback, null);
            }
            catch { }
        }

        public void SendQueueToServer()
        {
            if (!this.socket.Connected)
                return;

            //enqueue message and send immediately if only message in pipeline
            if (this.sendQueue.Any())
                this.SendToServer(this.sendQueue.Dequeue());
        }

        public void RequestSyncFromServer()
        {
            //request sync
            this.sendQueue.Enqueue(Message.Sync(Protocol.LOBBY));
            this.sendQueue.Enqueue(Message.Sync(Protocol.PROGRESS));
            this.SendQueueToServer();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                //end pending asynchronous connection request
                this.socket.EndConnect(ar);

                //attempt to login to server with user credentials
                string password  = Config.Network.Password;
                string uuid      = this.LocalUser.Id.String;
                string preferred = this.LocalUser.Name;
                string pronouns  = this.LocalUser.Pronouns;
                this.SendToServer(Message.LogIn(uuid, password, pronouns, preferred));
            }
            catch { }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                //finish sending bytes to server
                if (this.socket.Connected)
                    this.socket.EndSend(ar);
            }
            catch { }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                //end pending async read
                if (!this.socket.Connected)
                    return;

                int length = this.socket.EndReceive(ar);
                if (length > 0)
                {
                    byte[] bytes = Buffer.Take(length).ToArray();
                    string content = NetworkHelper.DecompressString(bytes);
                    var message = Message.FromString(content);

                    //process message
                    if (message.IsCommand)
                        this.ExecuteCommand(message);
                    else
                        this.CopyData(message);

                    //begin recieving again
                    if (this.socket.Connected)
                        this.socket.BeginReceive(Buffer, 0, Protocol.BUFFER_SIZE, SocketFlags.None, this.ReceiveCallback, null);

                    //send next queued message if any
                    if (this.sendQueue.Any())
                        this.SendToServer(this.sendQueue.Dequeue());
                }
            }
            catch
            {
                this.Stop("Lost connection to host.");
            }
        }


        private void ExecuteCommand(Message message)
        {
            if (!message.IsCommand)
                return;

            //return true if command was valid and executed fully
            if (message.Header is Protocol.ACCEPT)
            {
                //welcome message
                this.IsConnecting = false;
                message.TryGetItem(0, out string serverName);
                if (string.IsNullOrEmpty(serverName))
                    WriteToConsole($"Connected to remote host.");
                else
                    WriteToConsole($"Connected to {serverName}'s Tracker.");

                this.RequestSyncFromServer();

                //enable disconnect button
                UpdateControls("Disconnect", true, false);
                StateChangedFlag = true;
            }
            else if (message.Header is Protocol.REFUSE)
            {
                //connection refused by server 
                message.TryGetItem(0, out string reason);
                this.wasKickedByServer = true;
                this.Stop(reason);
            }
            else if (message.Header is Protocol.KICK)
            {
                //kicked from server 
                message.TryGetItem(0, out string reason);
                this.wasKickedByServer = true;
                this.Stop(reason);
            }
        }

        private void CopyData(Message message)
        {
            if (!message.IsData)
                return;

            if (message.Header is Protocol.LOBBY)
            {
                //deserialize lobby
                message.TryGetItem(0, out string jsonString);
                this.Lobby = Lobby.FromJsonString(jsonString);
                StateChangedFlag = true;
                SyncUserList(this.Lobby.Users.Values);

            }
            else if (message.TryGetItem(0, out string jsonString))
            {
                //deserialize progress
                this.recieved[message.Header] = jsonString;
                StateChangedFlag = true;
            }
            WriteToConsole($"Recieved {message.Header} from server.");
        }
    }
}
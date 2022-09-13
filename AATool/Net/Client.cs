using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Saves;

namespace AATool.Net
{
    public sealed class Client : Peer
    {
        public bool Accepted             { get; private set; }
        public bool IsConnecting         { get; private set; }
        public bool WasKickedByServer    { get; private set; }
        public bool LostConnection       { get; private set; }
        public bool DesignationsChanged  { get; private set; }
        public DateTime NextRefresh { get; private set; }

        private readonly Dictionary<string, string> received;
        private readonly Queue<Message> sendQueue;

        private IPEndPoint endPoint;
        private Socket socket;

        public Client() : base()
        {
            this.sendQueue = new ();
            this.received  = new ();
            this.IsConnecting = true;
        }

        public static bool TryGet(out Client client) => (client = Instance as Client) is not null;

        public override bool Connected() => !this.IsConnecting && this.socket?.Connected is true && this.Accepted;

        public bool TryGetData(string key, out string data) => this.received.TryGetValue(key, out data);

        public string GetLongStatusText()
        {
            string hostname = "remote server";
            if (this.Lobby.TryGetHost(out User host))
                hostname = host.Name;

            if (this.LostConnection)
                return $"Lost connection to {hostname}. Retrying...";
            if (!this.Connected())
                return "Attempting connection...";

            int remaining = (int)(this.NextRefresh - DateTime.UtcNow).TotalSeconds;
            if (remaining > 0)
            {
                string time = remaining >= 60
                    ? $"{remaining / 60} min & {remaining % 60} sec"
                    : $"{remaining} seconds";
                return $"Synced! Refreshing in {time}";
            }
            return $"Synced with {hostname}!";
        }

        public string GetShortStatusText()
        {
            int seconds = (int)Math.Ceiling((this.NextRefresh - DateTime.UtcNow).TotalSeconds);
            if (seconds <= 0)
            {
                string hostName = "host";
                if (TryGetLobby(out Lobby lobby) && lobby.TryGetHost(out User host))
                {
                    Player.TryGetName(host.Id, out string name);
                    hostName = name;
                }
                return $"Syncing with {hostName}";
            }
            else
            {
                return $"Refreshing in {Tracker.GetEstimateString(seconds).Replace(" ", "\0")}";
            }
        }


        protected override void Start(IPAddress address, int port, Uuid id)
        {
            base.Start(address, port, id);
            if (id == Uuid.Empty)
            {
                string reason = Player.ValidateName(Config.Net.MinecraftName)
                    ? "Couldn't get UUID. Either Mojang's servers didn't respond or an account with the requested name doesn't exist."
                    : "Invalid Minecraft name.";
                this.Stop("Error starting client: " + reason);
                return;
            }
            this.endPoint = new IPEndPoint(address, port);
            this.TryConnect();
        }

        private void TryConnect(bool retry = false)
        {
            this.Accepted = false;
            if (retry)
            {
                WriteToConsole("Reconnecting...");
                UpdateControls("Reconnecting...", false, false);
            }
            else
            {
                WriteToConsole("Attempting connection...");
                UpdateControls("Connecting...", false, false);
            }

            try
            {
                //attempt connection
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult ar = this.socket.BeginConnect(this.endPoint, this.ConnectCallback, this.socket);

                //set a timeout for connection attempt
                if (ar.AsyncWaitHandle.WaitOne(Protocol.Peers.ClientConnectMs, true) && this.socket.Connected)
                {
                    //start recieving messages from server 
                    this.socket.BeginReceive(Buffer, 0, Protocol.BufferSize, SocketFlags.None, this.ReceiveCallback, null);
                    this.IsConnecting = false;
                }
                else
                {
                    //couldn't establish connection with server
                    if (retry)
                        throw new TimeoutException();
                    else
                        this.Stop("Connection Timeout: Couldn't reach server.");
                }
            }
            catch (Exception exception)
            {
                this.socket?.Close();
                if (exception is SocketException or TimeoutException)
                {
                    if (retry)
                    {
                        UpdateControls("Stop", true, false);
                        Thread.Sleep(Protocol.Peers.ClientReconnectMs);
                        if (!this.IsDisposing)
                            this.TryConnect(true);
                        else
                            this.Stop("Stopped retrying.");
                        return;
                    }
                }
                this.Stop("A non-network error has occurred.");
            }
        }

        protected override void Stop(string reason)
        {
            this.IsConnecting = false;
            try
            {
                if (this.socket.Connected)
                {
                    if (!this.WasKickedByServer)
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
            StateChanged = true;
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
            catch (SocketException)
            {

            }
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
            this.sendQueue.Enqueue(Message.Sync(Protocol.Headers.Lobby));
            this.sendQueue.Enqueue(Message.Sync(Protocol.Headers.Progress));
            this.sendQueue.Enqueue(Message.Sync(Protocol.Headers.RefreshEstimate));
            this.SendQueueToServer();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                //end pending asynchronous connection request
                this.socket.EndConnect(ar);

                //attempt to login to server with user credentials
                string password  = Config.Net.Password;
                string uuid      = this.LocalUser.Id.String;
                string preferred = this.LocalUser.Name;
                string pronouns  = this.LocalUser.Pronouns;
                this.SendToServer(Message.LogIn(uuid, password, pronouns, preferred));
            }
            catch (SocketException)
            { 
            
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                //finish sending bytes to server
                if (this.socket.Connected)
                    this.socket.EndSend(ar);
            }
            catch (SocketException)
            {

            }
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
                    if (!NetworkHelper.TryDecompressString(bytes, out string content))
                        return;
                    var message = Message.FromString(content);

                    //process message
                    if (message.IsCommand)
                        this.ExecuteCommand(message);
                    else
                        this.CopyData(message);

                    //begin recieving again
                    if (this.socket.Connected)
                        this.socket.BeginReceive(Buffer, 0, Protocol.BufferSize, SocketFlags.None, this.ReceiveCallback, null);

                    //send next queued message if any
                    if (this.sendQueue.Any())
                        this.SendToServer(this.sendQueue.Dequeue());
                }
            }
            catch (SocketException)
            {
                this.LostConnection = true;
                this.TryConnect(true);
            }
        }

        private void ExecuteCommand(Message message)
        {
            if (!message.IsCommand)
                return;

            //return true if command was valid and executed fully
            if (message.Header is Protocol.Headers.Accept)
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
                StateChanged = true;
                this.Accepted = true;
                this.LostConnection = false;
            }
            else if (message.Header is Protocol.Headers.Refuse)
            {
                //connection refused by server 
                message.TryGetItem(0, out string reason);
                this.WasKickedByServer = true;
                this.Stop(reason);
            }
            else if (message.Header is Protocol.Headers.Kick)
            {
                //kicked from server 
                message.TryGetItem(0, out string reason);
                this.WasKickedByServer = true;
                this.Stop(reason);
            }
        }

        private void CopyData(Message message)
        {
            if (!message.IsData)
                return;

            if (message.Header is Protocol.Headers.Lobby)
            {
                //deserialize lobby
                message.TryGetItem(0, out string jsonString);
                this.Lobby = Lobby.FromJsonString(jsonString);
                StateChanged = true;
                SyncUserList(this.Lobby.Users.Values);

                foreach (KeyValuePair<string, Uuid> designation in this.Lobby.Designations)
                {
                    if (Tracker.TryGetAdvancement(designation.Key, out Advancement advancement) && advancement.DesignationLinked)
                        advancement.Designate(designation.Value);
                }
            }
            else if (message.Header is Protocol.Headers.RefreshEstimate)
            {
                //deserialize datetime
                message.TryGetItem(0, out string dateString);
                if (DateTime.TryParse(dateString, out DateTime nextEstimate))
                    this.NextRefresh = nextEstimate;
            }
            else if (message.Header is Protocol.Headers.BlockHighlights)
            {
                //deserialize all blocks highlights
                message.TryGetItem(0, out string list);
                if (Tracker.Category is AllBlocks ab)
                {
                    ab.ClearHighlighted();
                    ab.ClearConfirmed();
                    ab.ApplyChecklist(list?.Split('\n'));
                }
            }
            else if (message.TryGetItem(0, out string jsonString))
            {
                //deserialize progress
                this.received[message.Header] = jsonString;
                StateChanged = true;
            }
            WriteToConsole($"Received {message.Header} from server.");
        }
    }
}
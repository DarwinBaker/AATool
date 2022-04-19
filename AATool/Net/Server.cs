using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using AATool.Configuration;
using AATool.Net.Requests;
using AATool.Saves;

namespace AATool.Net
{
    public sealed class Server : Peer
    {
        private static readonly Dictionary<string, Uuid> PreparedDesignations = new ();

        private readonly Dictionary<string, Socket> clients = new ();
        private readonly Dictionary<Socket, User> users = new ();
        private readonly Socket listener;
        private readonly string password;

        private bool isStopping;

        public static void PrepareDesignation(string advancement, Uuid player) => PreparedDesignations[advancement] = player;

        public Server()
        {
            this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.password = Config.Net.Password;
            foreach (KeyValuePair<string, Uuid> designation in PreparedDesignations)
                this.DesignatePlayer(designation.Key, designation.Value);
        }

        public static bool TryGet(out Server server) => (server = Instance as Server) is not null;

        public override bool Connected() => this.listener.IsBound;

        public void SendToClient(Socket client, Message message)
        {
            if (!this.Connected() || !client.Connected)
                return;

            //start sending bytes to client socket
            //make sure socket exists
            byte[] compressed = NetworkHelper.CompressString(message.ToString());
            try
            {
                client.BeginSend(compressed, 0, compressed.Length, SocketFlags.None, this.SendCallback, client);
            }
            catch { }
        }

        public void SendToAllClients(Message message)
        {
            //send message to all connected client sockets
            foreach (Socket client in this.clients.Values.ToArray())
                this.SendToClient(client, message);
        }

        public void SendLobby(Socket client = null)
        {
            //clients never receive and are completely oblivious to everyone else's ip addresses
            
            string jsonString = this.Lobby.ToJsonString();
            var message = Message.Lobby(jsonString);

            //send lobby state to client(s)
            if (client is null)
                this.SendToAllClients(message);
            else
                this.SendToClient(client, message);
        }

        public void SendProgress(Socket client = null)
        {
            string jsonString = Tracker.State.ToJsonString();
            var message = Message.Progress(jsonString);

            //send lobby state to client(s)
            if (client is null)
                this.SendToAllClients(message);
            else
                this.SendToClient(client, message);
        }

        public void SendNextRefresh(Socket client = null)
        {
            var message = Message.SftpEstimate(MinecraftServer.GetRefreshEstimate());

            //send last to client(s)
            if (client is null)
                this.SendToAllClients(message);
            else
                this.SendToClient(client, message);
        }

        public void DesignatePlayer(string advancement, Uuid player)
        {
            if (!this.Lobby.Designations.TryGetValue(advancement, out Uuid current) || player != current)
            {
                this.Lobby.Designations[advancement] = player;
                this.SendLobby();
            }
        }

        public void KickPlayer(Uuid id)
        {
            foreach (KeyValuePair<Socket, User> user in this.users)
            {
                if (id == user.Value.Id)
                {
                    this.LogOut(user.Key, "forcibly kicked by host.");
                    break;
                }
            }
        }

        protected override void Start(IPAddress address, int port, Uuid id)
        {
            base.Start(address, port, id);

            //add host to lobby
            if (id != Uuid.Empty)
            {
                this.Lobby.SetHost(this.LocalUser);
                SyncUserList(this.Lobby.Users.Values);
            }
            else if (this.LocalUser != User.Nobody)
            {
                string reason = Player.ValidateName(Config.Net.MinecraftName)
                    ? "Unknown network error."
                    : "Invalid Minecraft name.";
                this.Stop("Failed: " + reason);
                return;
            }

            try
            {
                //create tcp listening socket and attempt binding
                this.listener.Bind(new IPEndPoint(address, port));
                this.listener.Listen(Protocol.Peers.ServerBacklog);

                //start accepting clients
                this.listener.BeginAccept(this.AcceptCallback, null);
                WriteToConsole("Started server.");
                WriteToConsole("Awaiting connections...");

                //disable controls
                UpdateControls("Stop", true, false);
                StateChanged = true;
            }
            catch (Exception e)
            {
                this.Stop($"Error starting server: {e.Message}.");
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            if (this.isStopping)
                return;
            try
            {
                //accept incoming attempt
                Socket client = this.listener.EndAccept(ar);
                client.BeginReceive(Buffer, 0, Protocol.BufferSize, SocketFlags.None, this.ReceiveCallback, client);
            }
            finally
            {
                try
                {
                    //listen for next connection attempt
                    this.listener.BeginAccept(this.AcceptCallback, null);
                }
                catch { }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is Socket client)
            {
                try
                {
                    //end pending async send
                    if (client.Connected)
                        client.EndSend(ar);
                }
                catch { }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is Socket client)
            {
                try
                {
                    //read data from client
                    int length = client.EndReceive(ar);
                    if (length > 0)
                    {
                        //reconstruct message
                        byte[] bytes = Buffer.Take(length).ToArray();
                        if (!NetworkHelper.TryDecompressString(bytes, out string content))
                            return;
                        var message = Message.FromString(content);

                        //process message
                        if (message.IsCommand)
                            this.ExecuteCommandAsync(message, client);
                    }
                    else
                    {
                        this.LogOut(client, "lost connection.");
                        return;
                    }

                    //start recieving again
                    if (client.Connected)
                        client.BeginReceive(Buffer, 0, Protocol.BufferSize, SocketFlags.None, this.ReceiveCallback, client);
                }
                catch
                {
                    this.LogOut(client, "lost connection.");
                }
            }
        }

        protected override void Stop(string reason)
        {
            this.isStopping = true;

            //close listener socket
            this.listener.Close();

            //disconnect clients
            if (this.clients.Any())
            {
                int total = this.clients.Count;
                foreach (Socket client in this.clients.Values.ToArray())
                {
                    this.LogOut(client, "Disconnected. (Server stopped)");
                    this.SendToClient(client, Message.Kick(reason));
                }
                WriteToConsole($"Disconnected {total} client{(total is 1 ? "" : "s")}.");
            }

            base.Stop(reason);
            UpdateControls("Host", true, true);
            StateChanged = true;
        }

        private static bool PasswordsAreEqual(string a, string b)
        {
            //time-constant password comparison (prevents timing attacks)
            uint difference = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                difference |= (uint)(a[i] ^ b[i]);
            return difference is 0;
        }

        private bool TryLogIn(User user, string password, string protocolVersion, Socket client)
        {
            //validate user credentials
            string problem = string.Empty;
            string name = user.Name;

            if (!Version.TryParse(protocolVersion, out Version version) || version != Protocol.Version)
                problem = "Client AATool version is not supported.";
            else if (!string.IsNullOrEmpty(this.password) && !PasswordsAreEqual(password, this.password))
                problem = "Incorrect password.";
            else if (this.clients.ContainsKey(client.RemoteEndPoint.ToString()))
                problem = "IP conflict.";
            else if (this.Lobby.Users.ContainsKey(user.Id))
                problem = "Minecraft username already taken.";
            else if (this.Lobby.Users.Values.Any(player => player.Name == name))
                problem = "Preferred username already taken.";
            else if (this.clients.Count is Protocol.Peers.ServerCapacity)
                problem = "Server full.";

            if (string.IsNullOrEmpty(problem))
            {
                //update currently connected clients
                this.Lobby.Add(user);
                this.SendLobby();

                //add client socket to list
                this.users[client] = user;
                this.clients[client.RemoteEndPoint.ToString()] = client;

                //log to console
                WriteToConsole($"{user.Name} connected!");
                SyncUserList(this.Lobby.Users.Values);

                //notify client that they have been accepted
                this.SendToClient(client, Message.Accept(this.LocalUser.Name));
                return true;
            }
            else
            {
                //refuse connection due to invalid user credentials
                problem = "Connection Refused: " + problem;
                this.SendToClient(client, Message.Refuse(problem));
                WriteToConsole(problem);
                return false;
            }
        }

        private void LogOut(Socket client, string reason, bool clientRequested = false)
        {
            if (this.clients.Count is 0 || this.isStopping)
                return;

            //log to console
            if (this.users.TryGetValue(client, out User user))
                WriteToConsole($"{user.Name} {reason}");
            else
                WriteToConsole(reason);

            if (client.Connected)
            {
                try
                {
                    //tell client to disconnect and dispose of socket
                    if (!clientRequested)
                        this.SendToClient(client, Message.Kick(reason));
                    client.Shutdown(SocketShutdown.Both);
                }
                catch { }

                client.Close();
            }
            
            //clear out client lists and lobby
            foreach (string ip in this.clients.Keys.ToArray())
            {
                if (this.clients[ip] == client)
                {
                    this.clients.Remove(ip);
                    break;
                }
            }
            this.users.Remove(client);
            this.Lobby.Remove(user);
            SyncUserList(this.Lobby.Users.Values);
            this.SendLobby();
        }

        private async void ExecuteCommandAsync(Message message, Socket sender)
        {
            if (message.Header is Protocol.Headers.Login)
            {
                message.TryGetItem(0, out string versionNumber);
                message.TryGetItem(1, out string uuid);
                message.TryGetItem(2, out string password);
                message.TryGetItem(3, out string pronouns);
                message.TryGetItem(4, out string displayName);

                if (!Uuid.TryParse(uuid, out Uuid id)) 
                {
                    WriteToConsole("Connection attempt refused: Malformed UUID.");
                    this.SendToClient(sender, Message.Refuse("Malformed UUID"));
                    return;
                }

                if (!await new NameRequest(id).DownloadAsync())
                {
                    WriteToConsole("Connection attempt refused: Couldn't validate name with Mojang.");
                    this.SendToClient(sender, Message.Refuse("Couldn't validate name with Mojang. Try re-connecting."));
                    return;
                }

                //register new user
                var user = new User(id, pronouns, displayName);
                if (this.TryLogIn(user, password, versionNumber, sender))
                    Player.FetchIdentity(id);
            }
            else if (message.Header is Protocol.Headers.Logout)
            {
                //remove player from lobby
                this.LogOut(sender, "disconnected.", true);
            }
            else if (message.Header is Protocol.Headers.Sync)
            {
                //send expected type of data to all clients
                message.TryGetItem(0, out string type);
                switch (type)
                {
                    case Protocol.Headers.Lobby:
                        this.SendLobby(sender);
                        break;
                    case Protocol.Headers.Progress:
                        this.SendProgress(sender);
                        break;
                    case Protocol.Headers.RefreshEstimate:
                        this.SendNextRefresh(sender);
                        break;
                }
            }
        }
    }
}

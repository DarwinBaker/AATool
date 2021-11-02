using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using AATool.Settings;

namespace AATool.Net
{
    public abstract partial class Peer
    {
        public static Peer Instance         { get; private set; }
        public static bool StateChangedFlag { get; protected set; }

        public static bool IsClient    => Instance is Client;
        public static bool IsServer    => Instance is Server;
        public static bool IsRunning   => Instance is not null;
        public static bool IsConnected => Instance?.Connected() ?? false;

        protected static readonly byte[] Buffer = new byte[Protocol.BufferSize];
        private static readonly List<INetworkController> Controllers = new ();

        public static void ClearFlags() => StateChangedFlag = false;

        public static bool TryGetLobby(out Lobby lobby)
        {
            if (Instance is not null)
            {
                lobby = Instance.Lobby;
                return true;
            }        
            lobby = null;
            return false;
        }

        public static void StartInstanceOf<T>(string ip, string port) where T : Peer, new()
        {
            //make sure not already running
            if (IsRunning)
                StopInstance();

            //make sure ip is valid
            bool requiresUser = typeof(T) == typeof(Client) || !string.IsNullOrEmpty(Config.Network.MinecraftName);
            if (requiresUser && !Player.ValidateName(Config.Network.MinecraftName))
            {
                string title = "Minecraft Name Formatting Error";
                string body  = $"The Minecraft name you entered is invalid. Minecraft names must be 3 to 16 characters in length, " +
                    "and only contain: a-Z, 0-9, and \"_\".";
                MessageBox.Show(body, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //make sure ip is valid
            if (!IPAddress.TryParse(ip, out IPAddress ipAddress))
            {
                string title = "IP Formatting Error";
                string body  = $"The IPv4 address you entered is invalid. IPv4 addresses must be in the format x.x.x.x, " +
                    "where each X is a number between 0 and 255.";
                MessageBox.Show(body, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //make sure port is valid
            if (!int.TryParse(port, out int portNumber))
            {
                string title = "Port Formatting Error";
                string body  = $"The port number you entered is invalid. Ports must be a number between 0 and 65535." +
                    "\n\nThe default and recommended port for this application is 25562.";

                MessageBox.Show(body, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //warn user if they are about to host without a password
            if (typeof(T) == typeof(Server) && string.IsNullOrEmpty(Config.Network.Password))
            {
                string title = "Unprotected Host Configuration";
                string body = "You are about to start hosting without a password!" +
                    "\n\nAll client IP addresses will still be completely safe, " +
                    "but anyone who has the host IP address will be able to connect, " +
                    "so unwanted parties may be able to join the lobby." +
                    "\n\nDo you still want to start hosting unprotected?";

                DialogResult result = MessageBox.Show(null, body, title, 
                    MessageBoxButtons.OKCancel, 
                    MessageBoxIcon.Warning, 
                    MessageBoxDefaultButton.Button2);

                if (result is not DialogResult.OK)
                    return;
            }

            //start peer instance as client or server without blocking main thread   
            new Thread(async () => 
            {
                Uuid id = Uuid.Empty;
                if (requiresUser)
                {
                    WriteToConsole("Getting UUID from Mojang...");
                    UpdateControls("Getting UUID...", false, false);
                    id = await Player.FetchUuidAsync(Config.Network.MinecraftName);
                }               
                Instance = new T();
                Instance.Start(ipAddress, portNumber, id);

            }) { IsBackground = true }.Start();
        }

        public static void StopInstance()
        {
            if (Instance is Client)
                Instance.Stop("Disconnected.");
            else if (Instance is Server)
                Instance.Stop("Server closed.");
        }

        public static void UpdateControls(string buttonText, bool enableButton, bool enableDropDown)
        {
            foreach (INetworkController controller in Controllers.ToArray())
                controller?.SetControlStates(buttonText, enableButton, enableDropDown);
        }
            

        public static void BindController(INetworkController controller)
        {
            Controllers.Add(controller);
            SyncConsole();
            if (IsRunning)
                SyncUserList(Instance.Lobby.Users.Values);
        }

        public static void UnbindController(INetworkController controller)
        {
            Controllers.Remove(controller);
        }

        public static void WriteToConsole(string line)
        {
            foreach (INetworkController controller in Controllers.ToArray())
            {
                try
                {
                    controller?.WriteToConsole(line);
                }
                catch { }
            }
            if (Instance is Peer peer)
                peer.ConsoleOutput += line + "\n";
        }

        public static void SyncConsole()
        {
            foreach (INetworkController controller in Controllers.ToArray())
            {
                try
                {
                    controller?.SyncConsole();
                }
                catch { }
            }
        }

        public static void SyncUserList(IEnumerable<User> users)
        {
            foreach (INetworkController controller in Controllers.ToArray())
            {
                try
                {
                    controller?.SyncUserList(users);
                }
                catch { }
            }
        }
    }
}

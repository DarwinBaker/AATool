using System.Net;
using AATool.Configuration;

namespace AATool.Net
{
    public abstract partial class Peer
    {
        protected Lobby Lobby           { get; set; }
        public User LocalUser           { get; private set; }
        public User Host                { get; private set; }
        public string Address           { get; private set; }
        public string Port              { get; private set; }      
        public string ConsoleOutput     { get; private set; }
        public bool IsDisposing         { get; private set; }

        public abstract bool Connected();

        public Peer()
        {
            this.Lobby = new();
        }

        protected virtual void Start(IPAddress address, int port, Uuid id) 
        {
            this.Address   = address.ToString();
            this.Port      = port.ToString();
            this.LocalUser = new User(id, Config.Net.Pronouns, Config.Net.PreferredName);
        }

        protected virtual void Stop(string reason)
        {
            Instance = null;
            this.IsDisposing = true;
            WriteToConsole(reason);
            SyncUserList(new User[0]);
        }
    }
}

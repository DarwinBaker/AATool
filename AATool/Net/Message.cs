using System;
using System.Collections.Generic;
using System.Linq;

namespace AATool.Net
{
    public readonly struct Message
    {
        public static readonly Message Empty = new ('\0', string.Empty);

        public static Message FromString(string message)
        {
            if (message is null)
                return Empty;

            string[] tokens = message.Split(Protocol.TokenDelimiter);
            if (tokens.Length > 0 && tokens[0].Length > 1)
            {
                char prefix    = tokens[0][0];
                string header  = tokens[0].Substring(1);
                string[] items = tokens.Skip(1).ToArray();
                return new Message(prefix, header, items);
            }
            return Empty;
        }

        public static Message LogIn(string uuid, string password, string pronouns, string displayName) => 
            NewCommand(Protocol.Headers.Login, Protocol.Version.ToString(), uuid, password, pronouns, displayName);
        public static Message LogOut()                        => NewCommand(Protocol.Headers.Logout);
        public static Message Sync(string type)               => NewCommand(Protocol.Headers.Sync, type);
        public static Message Accept(string serverHostName)   => NewCommand(Protocol.Headers.Accept, serverHostName);
        public static Message Refuse(string reason)           => NewCommand(Protocol.Headers.Refuse, reason);
        public static Message Kick(string reason)             => NewCommand(Protocol.Headers.Kick, reason);
        public static Message Progress(string jsonString)     => NewData(Protocol.Headers.Progress, jsonString);
        public static Message Lobby(string jsonString)        => NewData(Protocol.Headers.Lobby, jsonString);
        public static Message SftpEstimate(DateTime nextRefresh) => NewData(Protocol.Headers.RefreshEstimate, nextRefresh.ToString());

        private static Message NewCommand(string header, params string[] items) => new(Protocol.CommandPrefix, header, items);
        private static Message NewData(string header, string data) => new(Protocol.DataPrefix, header, data);

        public readonly char Prefix;
        public readonly string Header;
        public readonly string[] Items;

        private readonly string stringRepresentation;

        public bool IsCommand => this.Prefix is Protocol.CommandPrefix;
        public bool IsData    => this.Prefix is Protocol.DataPrefix;
        public bool IsEmpty   => this.Equals(Empty);

        private Message(char prefix, string header, params string[] items)
        {
            this.Prefix = prefix;
            this.Header = header;
            this.Items  = items;
            this.stringRepresentation = string.Concat(this.Prefix, 
                this.Header, 
                Protocol.TokenDelimiter.ToString(), 
                string.Join(Protocol.TokenDelimiter.ToString(), 
                this.Items));
        }

        public override string ToString()   => this.stringRepresentation;

        public bool TryGetItem(int index, out string item)
        {
            if (0 <= index && index < this.Items.Length)
            {
                item = this.Items[index];
                return true;
            }
            item = string.Empty;
            return false;
        }

        public override bool Equals(object obj) => 
            obj is Message message 
            && this.Prefix == message.Prefix
            && this.Header == message.Header
            && EqualityComparer<string[]>.Default.Equals(this.Items, message.Items);

        public override int GetHashCode()
        {
            int hashCode = -600507736;
            hashCode = (hashCode * -1521134295) + EqualityComparer<char>.Default.GetHashCode(this.Prefix);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.Header);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string[]>.Default.GetHashCode(this.Items);
            return hashCode;
        }
    }
}

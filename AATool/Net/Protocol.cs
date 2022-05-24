using System;

namespace AATool.Net
{
    public static class Protocol
    {
        public static readonly Version Version = new ("5.0");

        public const int BufferSize = 8192;
        public const char CommandPrefix = '/';
        public const char DataPrefix = '$';
        public const char TokenDelimiter = '\n';
        public const string HostKey = "$host";

        public static class Peers
        {
            public const int ClientReconnectMs = 3 * 1000;
            public const int ServerCapacity = 30;
            public const int ServerBacklog = 3;
            public const int DefaultPort = 25562;
        }

        public static class Requests
        {
            public const int MaxConcurrent = 3;
            public const int MaxRetries = 3;

            public const int TimeoutMs = 5 * 1000;
            public const double UpdateRate = 0.25;
            public const double RetryCooldown = 60;
        }

        public static class Headers
        {
            //client to server command headers
            public const string Login = "login";
            public const string Logout = "logout";
            public const string Sync = "sync";

            //server to client command headers
            public const string Accept = "accept";
            public const string Refuse = "refuse";
            public const string Kick = "kick";

            //server to client data headers
            public const string Progress = "progress";
            public const string Lobby = "lobby";
            public const string RefreshEstimate = "refresh estimate";
            public const string BlockHighlights = "block highlights";
        }
    }
}

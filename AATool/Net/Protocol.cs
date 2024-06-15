using System;

namespace AATool.Net
{
    public static class Protocol
    {
        public static readonly Version Version = new ("11.0");

        public const int BufferSize = 1024 * 1000;
        public const char CommandPrefix = '/';
        public const char DataPrefix = '$';
        public const char TokenDelimiter = '\n';
        public const string HostKey = "$host";

        public static class Peers
        {
            public const int ClientConnectMs = 10 * 1000;
            public const int ClientReconnectMs = 3 * 1000;
            public const int ServerCapacity = 30;
            public const int ServerBacklog = 3;
            public const int DefaultPort = 25562;
        }

        public static class Requests
        {
            public const int MaxConcurrent = 3;
            public const int MaxRetries = 2;

            public const int TimeoutNormalMs = 10 * 1000;
            public const int TimeoutLongerMs = 20 * 1000;
            public const double UpdateRate = 0.25;
            public const double RetryCooldown = 10 * 60 * 1000;
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

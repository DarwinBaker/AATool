using System;

namespace AATool.Net
{
    public static class Protocol
    {
        public static readonly Version Version = new ("2.0");

        //integer constants
        public const int CONNECTION_TIMEOUT_MS = 5 * 1000;
        public const int RECONNECT_COOLDOWN_MS = 3 * 1000;
        public const int UUID_REQUEST_LIMIT = 5;
        public const int SERVER_CAPACITY = 30;
        public const int SERVER_BACKLOG = 3;
        public const int BUFFER_SIZE = 8192;
        
        public const int MC_NAME_MIN = 3;
        public const int MC_NAME_MAX = 16;

        //requests
        public const int REQUEST_MAX_CONCURRENT = 3;
        public const int REQUEST_MAX_ATTEMPTS   = 3;
        public const double REQUEST_RETY_COOLDOWN  = 60;
        public const double REQUEST_NEXT_COOLDOWN  = 0.25;

        //message keywords
        public const char PREFIX_COMMAND    = '/';
        public const char PREFIX_DATA       = '$';
        public const char TOKEN_DELIM       = '\n';
        public const string HOST_KEY        = "$host";

        //client to server command headers
        public const string LOG_IN          = "login";
        public const string LOG_OUT         = "logout";
        public const string SYNC            = "sync";

        //server to client command headers
        public const string ACCEPT          = "accept";
        public const string REFUSE          = "refuse";
        public const string KICK            = "kick";

        //server to client data headers
        public const string PROGRESS         = "progress";
        public const string LOBBY            = "lobby";
        public const string REFRESH_ESTIMATE = "refresh estimate";
    }
}

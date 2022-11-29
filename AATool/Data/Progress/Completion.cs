using System;
using AATool.Net;

namespace AATool.Data.Progress
{
    public struct Completion
    {
        public static readonly Completion Empty = new (Uuid.Empty, default);

        public Uuid Player;
        public DateTime Timestamp;

        public bool IsEmpty => this.Player == Uuid.Empty;

        public Completion(Uuid player, DateTime timestamp)
        {
            this.Player = player;
            this.Timestamp = timestamp;
        }

        public bool Before(DateTime timestamp) => this.Timestamp < timestamp;
        public bool After(DateTime timestamp) => this.Timestamp > timestamp;
    }
}

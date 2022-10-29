using System;
using AATool.Net;
using Newtonsoft.Json;

namespace AATool.Data.Progress
{
    [JsonObject]
    public struct Completion
    {
        public static readonly Completion Empty = new (Uuid.Empty, default);

        [JsonProperty] public Uuid Player;
        [JsonProperty] public DateTime Timestamp;

        [JsonIgnore] public bool IsEmpty => this.Player == Uuid.Empty;

        public Completion(Uuid player, DateTime timestamp)
        {
            this.Player = player;
            this.Timestamp = timestamp;
        }

        public bool Before(DateTime timestamp) => this.Timestamp < timestamp;
        public bool After(DateTime timestamp) => this.Timestamp > timestamp;
    }
}

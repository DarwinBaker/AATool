using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AATool.Data.Progress
{
    [JsonObject]
    public class NetworkState
    {
        [JsonProperty] public List<NetworkContribution> Players { get; set; }
        [JsonProperty] public string GameCategory { get; set; }
        [JsonProperty] public string GameVersion { get; set; }
        [JsonProperty] public double KilometersFlown { get; set; }
        [JsonProperty] public int ItemsEnchanted { get; set; }
        [JsonProperty] public TimeSpan InGameTime { get; set; }

        [JsonConstructor]
        public NetworkState()
        {
            this.Players = new();
        }

        public NetworkState(WorldState state) : this()
        {
            //copy world state
            foreach (Contribution player in state.Players.Values)
                this.Players.Add(new NetworkContribution(player));

            //store current game category and version
            this.GameCategory = Tracker.CurrentCategory;
            this.GameVersion = Tracker.CurrentVersion;
            this.KilometersFlown = state.KilometersFlown;
            this.ItemsEnchanted = state.ItemsEnchanted;
            this.InGameTime = state.InGameTime;
        }

        public string ToJsonString() => JsonConvert.SerializeObject(this, Formatting.None);

        public static NetworkState FromJsonString(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<NetworkState>(jsonString);
            }
            catch
            {
                return new();
            }
        }
    }
}

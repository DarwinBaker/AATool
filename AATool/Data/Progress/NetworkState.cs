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

        [JsonConstructor]
        public NetworkState()
        {
            this.Players = new ();
        }

        public NetworkState(WorldState state) : this()
        {
            //copy world state
            foreach (Contribution player in state.Players.Values)
                this.Players.Add(new NetworkContribution(player));

            //store current game category and version
            this.GameCategory = Tracker.CurrentCategory;
            this.GameVersion = Tracker.CurrentVersion;
        }

        public static NetworkState FromJsonString(string jsonString) =>
            JsonConvert.DeserializeObject<NetworkState>(jsonString);

        public string ToJsonString() =>
            JsonConvert.SerializeObject(this);
    }
}

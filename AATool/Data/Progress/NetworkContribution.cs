using System;
using System.Collections.Generic;
using System.ComponentModel;
using AATool.Net;
using Newtonsoft.Json;

namespace AATool.Data.Progress
{
    [JsonObject]
    [TypeConverter(typeof(NetworkContribution))]
    public class NetworkContribution
    {
        [JsonProperty] public Uuid UUID { get; set; }
        [JsonProperty] public Dictionary<string, DateTime> Advancements { get; set; }
        [JsonProperty] public HashSet<string> Criteria { get; set; }

        [JsonProperty] public Dictionary<string, int> PickupCounts { get; set; }
        [JsonProperty] public Dictionary<string, int> DropCounts { get; set; }
        [JsonProperty] public Dictionary<string, int> MineCounts { get; set; }
        [JsonProperty] public Dictionary<string, int> CraftCounts { get; set; }
        [JsonProperty] public Dictionary<string, int> UseCounts { get; set; }
        [JsonProperty] public Dictionary<string, int> KillCounts { get; set; }

        [JsonProperty] public TimeSpan InGameTime { get; set; }
        [JsonProperty] public double KilometersFlown { get; set; }
        [JsonProperty] public bool ObtainedGodApple { get; set; }

        public NetworkContribution()
        {
            this.Advancements = new();
            this.Criteria = new();
            this.PickupCounts = new();
            this.DropCounts = new();
            this.MineCounts = new();
            this.CraftCounts = new();
            this.UseCounts = new();
            this.KillCounts = new();
        }

        public NetworkContribution(Contribution contribution) : this()
        {
            this.UUID = contribution.Player;
            this.InGameTime = contribution.InGameTime;
            this.ObtainedGodApple = contribution.ObtainedGodApple;

            //add advancements
            foreach (KeyValuePair<string, Completion> advancement in contribution.Advancements)
                this.Advancements[advancement.Key] = advancement.Value.Timestamp;
            //add criteria
            foreach (string criterion in contribution.Criteria.Keys)
                this.Criteria.Add(criterion);

            //add stats
            foreach (KeyValuePair<string, int> stat in contribution.PickupCounts)
                this.PickupCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in contribution.DropCounts)
                this.DropCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in contribution.MineCounts)
                this.MineCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in contribution.CraftCounts)
                this.CraftCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in contribution.UseCounts)
                this.UseCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in contribution.KillCounts)
                this.KillCounts[stat.Key] = stat.Value;
        }
    }
}

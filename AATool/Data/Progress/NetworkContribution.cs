using System;
using System.Collections.Generic;
using System.ComponentModel;
using AATool.Data.Categories;
using AATool.Data.Objectives;
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
        [JsonProperty] public HashSet<NetworkCriteriaSet> Multiparts { get; set; }

        [JsonProperty] public Dictionary<string, int> Pickup { get; set; }
        [JsonProperty] public Dictionary<string, int> Drop { get; set; }
        [JsonProperty] public Dictionary<string, int> Mine { get; set; }
        [JsonProperty] public Dictionary<string, int> Craft { get; set; }
        [JsonProperty] public Dictionary<string, int> Use { get; set; }
        [JsonProperty] public Dictionary<string, int> Kill { get; set; }

        [JsonProperty] public bool ObtainedGodApple { get; set; }

        [JsonIgnore]
        private static readonly HashSet<string> TrackedStats = new () {
            //items
            "minecraft:trident",
            "minecraft:nautilus_shell",
            "minecraft:enchanted_golden_apple",
            "minecraft:wither_skeleton_skull",
            "minecraft:ancient_debris",
            "minecraft:tnt",
            //kills
            "minecraft:creeper",
            "minecraft:drowned",
            "minecraft:wither_skeleton",
            "minecraft:phantom",
            "minecraft:cod",
            "minecraft:salmon",
            //misc stats
            "minecraft:bread",
            "minecraft:ender_pearl",
            "minecraft:netherrack",
            "minecraft:ender_chest",
            "minecraft:lectern",
            "minecraft:sugar_cane",
            "minecraft:conduit",
            "minecraft:bee_nest",
        };

        public NetworkContribution()
        {
            this.Advancements = new();
            this.Multiparts = new();
            this.Pickup = new();
            this.Drop = new();
            this.Mine = new();
            this.Craft = new();
            this.Use = new();
            this.Kill = new();
        }

        public NetworkContribution(Contribution contribution) : this()
        {
            this.UUID = contribution.Player;
            this.ObtainedGodApple = contribution.ObtainedGodApple;

            //add advancements
            foreach (KeyValuePair<string, Completion> advancement in contribution.Advancements)
                this.Advancements[advancement.Key] = advancement.Value.Timestamp;

            //add criteria
            var temp = new Dictionary<string, NetworkCriteriaSet>();
            foreach (string criterion in contribution.Criteria.Keys)
            {
                string[] tokens = criterion.Split(' ');
                if (tokens.Length is 2)
                {
                    string advancement = tokens[0];
                    if (!temp.TryGetValue(advancement, out NetworkCriteriaSet set))
                        temp[advancement] = set = new NetworkCriteriaSet(advancement);
                    set.List.Add(tokens[1]);
                }
            }
            foreach (NetworkCriteriaSet set in temp.Values)
                this.Multiparts.Add(set);

            //add stats
            foreach (KeyValuePair<string, int> stat in contribution.PickupCounts)
                this.TryAddStat(this.Pickup, stat);
            foreach (KeyValuePair<string, int> stat in contribution.DropCounts)
                this.TryAddStat(this.Drop, stat);
            foreach (KeyValuePair<string, int> stat in contribution.MineCounts)
                this.TryAddStat(this.Mine, stat);
            foreach (KeyValuePair<string, int> stat in contribution.CraftCounts)
                this.TryAddStat(this.Craft, stat);
            foreach (KeyValuePair<string, int> stat in contribution.UseCounts)
                this.TryAddStat(this.Use, stat);
            foreach (KeyValuePair<string, int> stat in contribution.KillCounts)
                this.TryAddStat(this.Kill, stat);
        }

        private void TryAddStat(Dictionary<string, int> counts, KeyValuePair<string, int> stat)
        {
            if (Tracker.Category is AllBlocks || TrackedStats.Contains(stat.Key))
                counts[stat.Key] = stat.Value;
        }
    }
}

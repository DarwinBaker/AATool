using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using AATool.Net;
using Newtonsoft.Json;

namespace AATool.Data.Progress
{
    [TypeConverter(typeof(Contribution))]
    [JsonObject]
    public class Contribution
    {
        [JsonProperty] public readonly Uuid PlayerId;
        [JsonProperty] public readonly HashSet<string> Advancements;
        [JsonProperty] public readonly HashSet<(string adv, string crit)> Criteria;
        [JsonProperty] public readonly Dictionary<string, int> ItemCounts;
        [JsonProperty] public readonly HashSet<string> BlocksPlaced;

        public int CompletedCount => this.Advancements.Count;
        
        [JsonConstructor]
        public Contribution(Uuid playerId, 
            HashSet<string> Advancements, 
            HashSet<(string, string)> Criteria, 
            Dictionary<string, int> ItemCounts, 
            HashSet<string> BlocksPlaced)
        {
            this.PlayerId      = playerId;
            this.Advancements  = Advancements;
            this.Criteria      = Criteria;
            this.ItemCounts    = ItemCounts;
            this.BlocksPlaced  = BlocksPlaced;
        }

        public Contribution(Uuid playerId)
        {
            this.PlayerId      = playerId;
            this.Advancements  = new();
            this.Criteria      = new();
            this.ItemCounts    = new();
            this.BlocksPlaced  = new();
        }

        public static Contribution FromJsonString(string jsonString) =>
            JsonConvert.DeserializeObject<Contribution>(jsonString);

        public string ToJsonString() => 
            JsonConvert.SerializeObject(this);

        public bool IncludesAdvancement(string id) => 
            this.Advancements.Contains(id);

        public bool IncludesBlock(string id) =>
            this.BlocksPlaced.Contains(id);

        public bool IncludesCriterion(string advancement, string criterion) => 
            this.Criteria.Contains((advancement, criterion));

        public int ItemCount(string item) => 
            this.ItemCounts.TryGetValue(item, out int val) ? val : 0;

        public void AddAdvancement(string advancement) =>
            this.Advancements.Add(advancement);

        public void AddCriterion(string advancement, string criterion) => 
            this.Criteria.Add((advancement, criterion));

        public void AddItemCount(string item, int count) =>
            this.ItemCounts[item] = count;

        public void AddBlock(string block) =>
            this.BlocksPlaced.Add(block);
    }
}

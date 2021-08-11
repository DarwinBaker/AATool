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
        [JsonProperty] public readonly Uuid Id;
        [JsonProperty] public readonly HashSet<string> Advancements;
        [JsonProperty] public readonly HashSet<(string adv, string crit)> Criteria;
        [JsonProperty] public readonly Dictionary<string, int> ItemCounts;

        public int CompletedCount => this.Advancements.Count;
        
        [JsonConstructor]
        public Contribution(Uuid Id, 
            HashSet<string> Advancements, 
            HashSet<(string, string)> Criteria, Dictionary<string, int> ItemCounts)
        {
            this.Id            = Id;
            this.Advancements  = Advancements;
            this.Criteria      = Criteria;
            this.ItemCounts    = ItemCounts;
        }

        public Contribution(Uuid playerId)
        {
            this.Id            = playerId;
            this.Advancements  = new();
            this.Criteria      = new();
            this.ItemCounts    = new();
        }

        public static Contribution FromJsonString(string jsonString) =>
            JsonConvert.DeserializeObject<Contribution>(jsonString);

        public string ToJsonString() => JsonConvert.SerializeObject(this);

        public bool IncludesAdvancement(string id) => 
            this.Advancements.Contains(id);

        public bool IncludesCriterion(string advancement, string criterion) => 
            this.Criteria.Contains((advancement, criterion));

        public int ItemCount(string item) => 
            this.ItemCounts.TryGetValue(item, out int val) ? val : 0;

        public void AddItemCount(string item, int count) => 
            this.ItemCounts[item] = count;

        public void AddAdvancement(string advancement)
        {
            //add advancement to list of advancements completed by this user
            if (!this.Advancements.Contains(advancement))
                this.Advancements.Add(advancement);
        }

        public void AddCriterion(string advancement, string criterion)
        {
            //add criterion to list of criteria completed by this user
            this.Criteria.Add((advancement, criterion));
        }
    }
}

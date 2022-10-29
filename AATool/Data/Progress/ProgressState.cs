using System;
using System.Collections.Generic;
using System.ComponentModel;
using AATool.Data.Objectives;
using Newtonsoft.Json;

namespace AATool.Data.Progress
{
    [TypeConverter(typeof(ProgressState))]
    [JsonObject]
    public abstract class ProgressState
    {
        [JsonProperty] public Dictionary<string, Completion> Advancements { get; set; }
        [JsonProperty] public Dictionary<string, Completion> Recipes { get; set; }
        [JsonProperty] public Dictionary<(string adv, string crit), Completion> Criteria { get; set; }
        [JsonProperty] public HashSet<string> DeathMessages { get; set; }

        [JsonProperty] public Dictionary<string, int> PickupCounts { get; set; }
        [JsonProperty] public Dictionary<string, int> DropCounts   { get; set; }
        [JsonProperty] public Dictionary<string, int> MineCounts   { get; set; }
        [JsonProperty] public Dictionary<string, int> CraftCounts  { get; set; }
        [JsonProperty] public Dictionary<string, int> UseCounts    { get; set; }
        [JsonProperty] public Dictionary<string, int> KillCounts   { get; set; }

        [JsonProperty] public TimeSpan InGameTime { get; set; }

        [JsonProperty] public double KilometersFlown { get; set; }

        [JsonProperty] public bool ObtainedGodApple { get; set; }

        [JsonProperty] public int Deaths { get; set; }
        [JsonProperty] public int DamageTaken { get; set; }
        [JsonProperty] public int DamageDealt { get; set; }
        [JsonProperty] public int Jumps { get; set; }
        [JsonProperty] public int Sleeps { get; set; }
        [JsonProperty] public int SaveAndQuits { get; set; }
        [JsonProperty] public int ItemsEnchanted { get; set; }

        [JsonConstructor]
        public ProgressState(Dictionary<string, Completion> Advancements,
            Dictionary<(string, string), Completion> Criteria,
            Dictionary<string, Completion> Recipes)
        {
            this.Advancements = Advancements;
            this.Criteria = Criteria;
            this.Recipes = Recipes;
        }

        public ProgressState()
        {
            this.Advancements = new ();
            this.Criteria = new ();
            this.Recipes = new ();
            this.DeathMessages = new ();
            this.PickupCounts = new ();
            this.DropCounts = new ();
            this.MineCounts = new ();
            this.CraftCounts = new ();
            this.UseCounts = new ();
            this.KillCounts = new ();
        }
        public abstract HashSet<Completion> CompletionsOf(IObjective objective);

        public bool AdvancementCompleted(string id) =>
            this.Advancements.ContainsKey(id);

        public bool CriterionCompleted(string advancement, string criterion) =>
            this.Criteria.ContainsKey((advancement, criterion));

        public bool WasPickedUp(string name) =>
            this.PickupCounts.ContainsKey(name);

        public bool WasDropped(string name) =>
            this.DropCounts.ContainsKey(name);

        public bool WasMined(string name) =>
            this.MineCounts.ContainsKey(name);

        public bool WasCrafted(string name) =>
            this.CraftCounts.ContainsKey(name);

        public bool WasUsed(string name) =>
            this.UseCounts.ContainsKey(name);

        public bool WasKilled(string name) =>
            this.KillCounts.ContainsKey(name);

        public int TimesPickedUp(string name) =>
            this.PickupCounts.TryGetValue(name, out int count) ? count : 0;

        public int TimesDropped(string name) =>
            this.DropCounts.TryGetValue(name, out int count) ? count : 0;

        public int TimesMined(string name) =>
            this.MineCounts.TryGetValue(name, out int count) ? count : 0;

        public int TimesCrafted(string name) =>
            this.CraftCounts.TryGetValue(name, out int count) ? count : 0;

        public int TimesUsed(string name) =>
            this.UseCounts.TryGetValue(name, out int count) ? count : 0;

        public int TimesKilled(string name) =>
            this.KillCounts.TryGetValue(name, out int count) ? count : 0;

    }
}

using System;
using System.Collections.Generic;
using AATool.Data.Objectives;

namespace AATool.Data.Progress
{
    public abstract class ProgressState
    {
        public Dictionary<string, Completion> Advancements { get; set; }
        public Dictionary<string, Completion> Criteria { get; set; }
        public Dictionary<string, Completion> Recipes { get; set; }
        public HashSet<string> DeathMessages { get; set; }

        public Dictionary<string, int> PickupCounts { get; set; }
        public Dictionary<string, int> DropCounts   { get; set; }
        public Dictionary<string, int> MineCounts   { get; set; }
        public Dictionary<string, int> CraftCounts  { get; set; }
        public Dictionary<string, int> UseCounts    { get; set; }
        public Dictionary<string, int> KillCounts   { get; set; }

        public TimeSpan InGameTime { get; set; }

        public double KilometersFlown { get; set; }

        public bool ObtainedGodApple { get; set; }
        public bool ObtainedHeavyCore { get; set; }
        public bool ObtainedLapis { get; set; }

        public int Deaths { get; set; }
        public int DamageTaken { get; set; }
        public int DamageDealt { get; set; }
        public int Jumps { get; set; }
        public int Sleeps { get; set; }
        public int SaveAndQuits { get; set; }
        public int ItemsEnchanted { get; set; }

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
            this.Criteria.ContainsKey(Criterion.Key(advancement, criterion));

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

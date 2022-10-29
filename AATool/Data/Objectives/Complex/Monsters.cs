
using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class Monsters : ComplexCriteriaObjective
    {
        private static readonly string[] RaidMobs = new string[] {
            "minecraft:ravager", "minecraft:vex", "minecraft:evoker",
            "minecraft:witch", "minecraft:vindicator", "minecraft:pillager",
        };

        private bool OnlyRaidMobsLeft =>
            this.RemainingNonRaidMobs.Count is 0
            && this.RemainingCriteria.Count <= RaidMobs.Length
            && this.RemainingCriteria.Count > 0;

        private bool OnlyRaidMobsPlusOneLeft =>
            this.RemainingNonRaidMobs.Count is 1 
            && this.RemainingCriteria.Count > 1;

        protected readonly HashSet<string> RemainingNonRaidMobs = new ();

        public Monsters() : base()
        {
            this.Name = "Monsters";
        }

        public override string AdvancementId => "minecraft:adventure/kill_all_mobs";
        public override string Criterion => "Mob";
        public override string Action => "Kill";
        public override string PastAction => "Killed";
        protected override string ModernTexture => "kill_all_mobs";
        protected override string OldTexture => "kill_all_mobs_1.12";

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            base.UpdateAdvancedState(progress);
        }

        protected override void BuildRemainingCriteriaList(CriteriaSet criteria)
        {
            this.CurrentCriteria = 0;
            this.RequiredCriteria = criteria.Count;
            this.RemainingCriteria.Clear();
            this.RemainingNonRaidMobs.Clear();
            foreach (Criterion criterion in criteria.All.Values)
            {
                if (criterion.IsComplete())
                {
                    this.CurrentCriteria++;
                }
                else
                {
                    _= this.RemainingCriteria.Add(criterion.Name);
                    if (!RaidMobs.Contains(criterion.Id))
                        this.RemainingNonRaidMobs.Add(criterion.Name);
                    this.LastCriterionIcon = criterion.Icon;
                }
            }
        }

        protected override void ClearAdvancedState()
        {
            this.RemainingNonRaidMobs.Clear();
            base.ClearAdvancedState();
        }

        protected override string GetLongStatus()
        {
            if (this.CompletionOverride)
                return this.LongStatusComplete();

            if (this.OnLastCriterion)
                return this.LongStatusLast();

            if (this.OnlyRaidMobsLeft)
                return $"Awaiting\nRaid";

            if (this.OnlyRaidMobsPlusOneLeft)
                return $"Needs\0Raid\n&\0{this.FormatMobName(this.RemainingNonRaidMobs.First())}";   

            return this.LongStatusNormal();
        }

        private string FormatMobName(string name)
        {
            return name.Replace("Ender Dragon", "Dragon")
                .Replace("Zombie Villager", "Zillager")
                .Replace("Zombie Piglin", "Ziglin")
                .Replace("Piglin Brute", "Brute");
        }

        protected override string LongStatusNormal() =>
            $"Mobs\0Killed\n{this.CurrentCriteria}\0/\0{this.RequiredCriteria}";

        protected override void RefreshIcon()
        {
            if (!UseModernTexture)
                this.Icon = this.OldTexture;
            else if (this.CompletionOverride || this.RemainingCriteria.Count is 0)
                this.Icon = "enchanted_diamond_sword";
            else if (this.OnLastCriterion)
                this.Icon = this.LastCriterionIcon;
            else if (this.OnlyRaidMobsLeft || this.OnlyRaidMobsPlusOneLeft)
                this.Icon = "enchanted_diamond_sword";
            else
                this.Icon = this.ModernTexture;
        }
    }
}

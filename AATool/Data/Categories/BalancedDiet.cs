using System.Collections.Generic;
using AATool.Data.Objectives;

namespace AATool.Data.Categories 
{
    public class BalancedDiet : SingleAdvancement
    {
        private const string Id = "minecraft:husbandry/balanced_diet";

        public static readonly List<string> SupportedVersions = new () {
            "1.17",
            "1.16",
        };

        public BalancedDiet() : base()
        {
            this.Name      = "Balanced Diet";
            this.Acronym   = "ABD";
            this.Objective = "Foods";
            this.Action    = "Eaten";
        }

        public override int GetTargetCount()
        {
            return Tracker.TryGetAdvancement(Id, out this.Advancement)
                ? this.Advancement.Criteria.Count
                : 0;
        }

        public override int GetCompletedCount()
        {
            return Tracker.TryGetAdvancement(Id, out this.Advancement)
                ? this.Advancement.Criteria.MostCompleted
                : 0;
        }

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
    }
}

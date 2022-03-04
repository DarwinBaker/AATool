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

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;

        public BalancedDiet() : base()
        {
            this.Name      = "Balanced Diet";
            this.Acronym   = "ABD";
            this.Objective = "Foods";
            this.Action    = "Eaten";
        }

        public override void LoadObjectives()
        {
            this.Advancements.RefreshObjectives();
            this.Advancements.TryGet(Id, out Advancement balancedDiet);
            this.Requirement = balancedDiet;
        }
    }
}

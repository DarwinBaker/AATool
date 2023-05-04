using System.Collections.Generic;
using System.Xml;
using AATool.Data.Objectives;
using AATool.Data.Objectives.Complex;
using AATool.Utilities;

namespace AATool.Data.Categories 
{
    public class AllSmithingTemplates : SingleAdvancement
    {
        private const string Id = "custom:all_smithing_templates";

        public static readonly List<string> SupportedVersions = new () {
            "1.20 Snapshot",
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
        public override int GetCompletedCount() => this.RecipesObtained;
        
        public int RecipesObtained { get; private set; }

        public AllSmithingTemplates() : base()
        {
            this.Name      = "All Smithing Templates";
            this.Acronym   = "AST";
            this.Objective = "Templates";
            this.Action    = "Obtained";
        }

        public override void LoadObjectives()
        {
            Tracker.Advancements.RefreshObjectives();
            Tracker.Advancements.TryGet(Id, out Advancement allSmithingTemplates);
            this.Requirement = allSmithingTemplates;
        }

        public override void Update()
        {
            base.Update();
            RecipesObtained = 0;
            if (this.Requirement?.Criteria is not CriteriaSet trims)
                return;

            foreach (ArmorTrimCriterion criterion in trims.All.Values)
            {
                if (criterion.Obtained)
                    RecipesObtained++;
            }
        }
    }
}

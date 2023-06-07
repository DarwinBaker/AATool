using System.Collections.Generic;
using AATool.Data.Objectives;

namespace AATool.Data.Categories 
{
    public class AllAdvancements : Category
    {
        public static readonly List<string> SupportedVersions = new () {
            "1.20",
            "1.19",
            "1.18",
            "1.17",
            "1.16.5",
            "1.16",
            "1.15",
            "1.14",
            "1.13",
            "1.12",
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
        public override IEnumerable<Objective> GetOverlayObjectives() => Tracker.Advancements.AllAdvancements.Values;

        public override string GetDefaultVersion() => "1.16";

        public override int GetCompletedCount() => Tracker.Advancements.CombinedCompletedCount;
        public override int GetTargetCount() => Tracker.Advancements.Count;

        public AllAdvancements() : base ()
        {
            this.Name      = "All Advancements";
            this.Acronym   = "AA";
            this.Objective = "Advancements";
            this.Action    = "Complete";
        }

        public override void LoadObjectives()
        {
            Tracker.Advancements.RefreshObjectives();
            Tracker.ComplexObjectives.RefreshObjectives();
        }
    }
}

using System.Collections.Generic;

namespace AATool.Data.Categories 
{
    public class AllAdvancements : Category
    {
        public static readonly List<string> SupportedVersions = new () {
            "1.18",
            "1.17",
            "1.16.5",
            "1.16",
            "1.15",
            "1.14",
            "1.13",
            "1.12",
        };

        public AllAdvancements() : base ()
        {
            this.Name      = "All Advancements";
            this.Acronym   = "AA";
            this.Objective = "Advancements";
            this.Action    = "Complete";
        }

        public override int GetTargetCount() => Tracker.Advancements.Count;
        public override int GetCompletedCount() => Tracker.Advancements.CompletedCount;
        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
    }
}

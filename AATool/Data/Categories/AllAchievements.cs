
using System.Collections.Generic;

namespace AATool.Data.Categories
{
    public class AllAchievements : Category
    {
        public static readonly HashSet<string> SupportedVersions = new () {
            "1.11",
        };

        public AllAchievements() : base ()
        {
            this.Name      = "All Achievements";
            this.Acronym   = "AACH";
            this.Objective = "Achievements";
            this.Action    = "Complete";
        }

        public override int GetTargetCount() => Tracker.Achievements.Count;
        public override int GetCompletedCount() => Tracker.Achievements.CompletedCount;
        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
    }
}

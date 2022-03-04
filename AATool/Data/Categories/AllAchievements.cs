
using System.Collections.Generic;
using AATool.Data.Objectives;

namespace AATool.Data.Categories
{
    public class AllAchievements : Category
    {
        public static readonly HashSet<string> SupportedVersions = new () {
            "1.11",
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
        public override IEnumerable<Objective> GetOverlayObjectives() => this.Achievements.All.Values;

        public override int GetTargetCount() => this.Achievements.Count;
        public override int GetCompletedCount() => this.Achievements.CompletedCount;

        public AllAchievements() : base ()
        {
            this.Name      = "All Achievements";
            this.Acronym   = "AACH";
            this.Objective = "Achievements";
            this.Action    = "Complete";
        }

        public override void LoadObjectives()
        {
            this.Achievements.RefreshObjectives();
            this.Pickups.RefreshObjectives();
        }
    }
}

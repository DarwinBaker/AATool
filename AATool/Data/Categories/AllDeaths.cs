using System.Collections.Generic;
using AATool.Data.Objectives;
using AATool.Graphics;

namespace AATool.Data.Categories
{
    public class AllDeaths : Category
    {
        public static readonly List<string> SupportedVersions = new () {
            "1.17",
            "1.16",
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
        public override IEnumerable<Objective> GetOverlayObjectives() => Tracker.Deaths.All.Values;

        public override int GetTargetCount() => Tracker.Deaths.Count;
        public override int GetCompletedCount() => Tracker.Deaths.TotalExperienced;

        public AllDeaths() : base()
        {
            this.Name      = "All Deaths";
            this.Acronym   = "AD";
            this.Objective = "Deaths";
            this.Action    = "Experienced";

            SpriteSheet.Require("deaths");
        }


        public override void LoadObjectives()
        {
            Tracker.Deaths.RefreshObjectives();
            Tracker.Pickups.RefreshObjectives();
        }
    }
}

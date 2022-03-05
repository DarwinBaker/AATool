using System.Collections.Generic;
using AATool.Data.Objectives;
using AATool.Graphics;

namespace AATool.Data.Categories
{
    public class AllBlocks : Category
    {
        public static readonly List<string> SupportedVersions = new () {
            "1.18"
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
        public override IEnumerable<Objective> GetOverlayObjectives() => Tracker.Blocks.All.Values;

        public override int GetTargetCount() => Tracker.Blocks.Count;
        public override int GetCompletedCount() => Tracker.Blocks.PlacedCount;

        public AllBlocks() : base()
        {
            this.Name      = "All Blocks";
            this.Acronym   = "AB";
            this.Objective = "Blocks";
            this.Action    = "Placed";

            SpriteSheet.Require("blocks");
        }

        public override void LoadObjectives()
        {
            Tracker.Blocks.RefreshObjectives();
            Tracker.Pickups.RefreshObjectives();
        }
    }
}

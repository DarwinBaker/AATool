using System.Collections.Generic;

namespace AATool.Data.Categories
{
    public class AllBlocks : Category
    {
        public static readonly List<string> SupportedVersions = new () {
            "1.18"
        };

        public AllBlocks() : base()
        {
            this.Name      = "All Blocks";
            this.Acronym   = "AB";
            this.Objective = "Blocks";
            this.Action    = "Placed";
        }

        public override int GetTargetCount() => Tracker.Blocks.Count;
        public override int GetCompletedCount() => Tracker.Blocks.PlacedCount;
        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
    }
}

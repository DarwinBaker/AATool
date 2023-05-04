using System.Collections.Generic;
using AATool.Data.Objectives;

namespace AATool.Data.Categories 
{
    public class AdventuringTime : SingleAdvancement
    {
        private const string Id = "minecraft:adventure/adventuring_time";

        public static readonly List<string> SupportedVersions = new () {
            "1.20 Snapshot",
            "1.19",
            "1.18",
            "1.16",
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;

        public AdventuringTime() : base()
        {
            this.Name      = "Adventuring Time";
            this.Acronym   = "AT";
            this.Objective = "Biomes";
            this.Action    = "Visited";
        }

        public override void LoadObjectives()
        {
            Tracker.Advancements.RefreshObjectives();
            Tracker.Advancements.TryGet(Id, out Advancement adventuringTime);
            this.Requirement = adventuringTime;
        }
    }
}

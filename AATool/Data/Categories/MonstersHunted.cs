using System.Collections.Generic;
using AATool.Data.Objectives;

namespace AATool.Data.Categories 
{
    public class MonstersHunted : SingleAdvancement
    {
        private const string Id = "minecraft:adventure/kill_all_mobs";

        public static readonly List<string> SupportedVersions = new () {
            "1.16.5",
            "1.16",
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;

        public MonstersHunted() : base()
        {
            this.Name      = "Monsters Hunted";
            this.Acronym   = "MH";
            this.Objective = "Monsters";
            this.Action    = "Killed";
        }

        public override void LoadObjectives()
        {
            this.Advancements.RefreshObjectives();
            this.Advancements.TryGet(Id, out Advancement monstersHunted);
            this.Requirement = monstersHunted;
        }
    }
}

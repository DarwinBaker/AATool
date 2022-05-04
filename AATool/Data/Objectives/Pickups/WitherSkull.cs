using System.Xml;
using AATool.Data.Categories;

namespace AATool.Data.Objectives.Pickups
{
    class WitherSkull : Pickup
    {
        public const string ItemId = "minecraft:wither_skeleton_skull";
        public const string LegacyItemId = "minecraft.skull";
        private const string MonsterHunter = "minecraft:adventure/kill_all_mobs";
        private const string Wither = "minecraft:wither";
        private const string WitherRose = "minecraft:wither_rose";
        private const string Beacon = "minecraft:beacon";

        private bool witherRosePlaced;
        private bool beaconPlaced;

        public WitherSkull(XmlNode node) : base(node) 
        {
            if (Tracker.Category is AllBlocks)
            {
                this.Icon = "skull_and_beacon";
                this.TargetCount = 4;
            }
        }

        protected override void HandleCompletionOverrides()
        {
            if (Tracker.Category is AllBlocks)
            {
                //check if wither rose has been placed
                Tracker.TryGetBlock(WitherRose, out Block witherRose);
                this.witherRosePlaced = witherRose?.CompletedByAnyone() is true;
                Tracker.TryGetBlock(Beacon, out Block beacon);
                this.beaconPlaced = beacon?.CompletedByAnyone() is true;

                this.CompletionOverride = this.witherRosePlaced && this.beaconPlaced;
            }
            else
            {
                //check if wither has been killed
                Tracker.TryGetCriterion(MonsterHunter, Wither, out Criterion killWither);
                this.CompletionOverride = killWither?.CompletedByDesignated() is true;
            }
        }

        protected override void UpdateLongStatus()
        {
            //show wither killed if applicable
            if (this.CompletionOverride)
            {
                this.FullStatus = Tracker.Category is AllBlocks 
                    ? "Beacon&Rose Placed" 
                    : "Wither Has Been Killed";
            }
            else
            { 
                base.UpdateLongStatus();
            }
        }
    }
}
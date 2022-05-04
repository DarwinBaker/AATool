using System.Xml;
using AATool.Data.Categories;

namespace AATool.Data.Objectives.Pickups
{
    class GoldBlocks : Pickup
    {
        public const string ItemId = "minecraft:gold_block";
        public const string LegacyItemId = "minecraft.gold_block";
        public const string GoldIngotId = "minecraft:gold_ingot";
        public const int IngotsPerBlock = 9;

        private const string Beaconator = "minecraft:nether/create_full_beacon";
        private const string LegacyBeaconator = "achievement.fullBeacon";

        public GoldBlocks(XmlNode node) : base(node) { }

        /*
        public override int GetTotal()
        { 
            int goldBlocks = base.GetTotal();
            goldBlocks += Tracker.State.GoldIngotsPickedUp / IngotsPerBlock;
            goldBlocks -= Tracker.State.GoldIngotsDropped / IngotsPerBlock;
            return goldBlocks;
        }
        */

        protected override void HandleCompletionOverrides()
        {
            //ignore count if full beacon has been constructed
            Advancement beaconator;
            if (Tracker.Category is AllAchievements)
                Tracker.TryGetAdvancement(LegacyBeaconator, out beaconator);
            else
                Tracker.TryGetAdvancement(Beaconator, out beaconator);
            this.CompletionOverride = beaconator?.IsComplete() is true;
        }

        protected override void UpdateLongStatus()
        {
            if (this.CompletionOverride)
                this.FullStatus = "Full Beacon Constructed";
            else
                base.UpdateLongStatus();
        }
    }
}

using System;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Complex
{
    public class WitherSkulls : ComplexObjective
    {
        public const string ItemId = "minecraft:wither_skeleton_skull";
        public const string LegacyItemId = "minecraft.skull";
        private const string MonsterHunter = "minecraft:adventure/kill_all_mobs";
        private const string Wither = "minecraft:wither";
        private const string WitherRose = "minecraft:wither_rose";
        private const string Beacon = "minecraft:beacon";

        private const string Beaconator = "minecraft:nether/create_full_beacon";
        private const string LegacyBeaconator = "achievement.fullBeacon";

        private static readonly Version BlockIdChanged = new ("1.13");

        private static bool UseModernId => !Version.TryParse(Tracker.CurrentVersion, out Version current)
            || current >= BlockIdChanged;

        public int EstimatedObtained { get; private set; }

        private bool fullBeaconComplete;

        private bool roseObtained;
        private bool rosePlaced;
        private bool beaconCrafted;
        private bool beaconPlaced;
        private bool witherKilled;
        private bool skullsPlaced;

        public int Required => Tracker.Category is AllBlocks ? 4 : 3;

        public WitherSkulls() : base() 
        {
            this.Name = "WitherSkulls";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            string itemId = UseModernId ? ItemId : LegacyItemId;

            this.EstimatedObtained = progress.TimesPickedUp(itemId)
                - progress.TimesDropped(itemId)
                - progress.TimesUsed(itemId);
            this.EstimatedObtained = Math.Max(0, this.EstimatedObtained);

            this.fullBeaconComplete = Tracker.Category is AllAchievements
                ? progress.AdvancementCompleted(LegacyBeaconator)
                : progress.AdvancementCompleted(Beaconator);

            this.skullsPlaced = progress.WasUsed(itemId);

            //check wither rose status
            this.roseObtained = progress.WasPickedUp(WitherRose);
            this.rosePlaced = progress.WasUsed(WitherRose);

            //check beacon status
            this.beaconCrafted = progress.WasCrafted(Beacon);
            this.beaconPlaced = progress.WasUsed(Beacon);

            //check beacon status
            this.witherKilled = progress.CriterionCompleted(MonsterHunter, Wither);

            if (Tracker.Category is AllBlocks)
            {
                this.CompletionOverride = this.EstimatedObtained >= this.Required || this.skullsPlaced
                    || this.witherKilled || this.rosePlaced || this.beaconPlaced;
            }
            else
            {
                this.CompletionOverride = this.EstimatedObtained >= this.Required || this.skullsPlaced
                    || this.fullBeaconComplete || this.witherKilled;
            }
        }

        protected override void ClearAdvancedState()
        {
            this.EstimatedObtained = 0;
            this.roseObtained = false;
            this.rosePlaced = false;
            this.beaconCrafted = false;
            this.beaconPlaced = false;
            this.witherKilled = false;
        }

        protected override string GetShortStatus() => $"{this.EstimatedObtained} / {Required}";

        protected override string GetLongStatus()
        {
            if (Tracker.Category is AllBlocks)
            {
                if (this.beaconPlaced && this.rosePlaced)
                    return "Beacon+Rose\nPlaced";
            }

            return this.witherKilled 
                ? "Wither\0Has\nBeen\0Killed" 
                : $"Skulls\n{this.EstimatedObtained}\0/\0{this.Required}";
        }

        protected override string GetCurrentIcon()
        {
            if (Tracker.Category is AllBlocks && this.beaconPlaced && this.rosePlaced)
                return "skull_and_beacon";

            return "get_wither_skull";
        }
    }
}
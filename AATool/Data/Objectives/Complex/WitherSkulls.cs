using System;
using System.Xml;
using AATool.Configuration;
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
        private const string SummonWither = "minecraft:nether/summon_wither";
        private const string Wither = "minecraft:wither";
        private const string WitherSkeleton = "minecraft:wither_skeleton";
        private const string WitherRose = "minecraft:wither_rose";
        private const string Beacon = "minecraft:beacon";

        private const string Beaconator = "minecraft:nether/create_full_beacon";
        private const string LegacyBeaconator = "achievement.fullBeacon";

        private static readonly Version BlockIdChanged = new ("1.13");

        private static bool UseModernId => !Version.TryParse(Tracker.CurrentVersion, out Version current)
            || current >= BlockIdChanged;

        public int EstimatedObtained { get; private set; }

        private bool fullBeaconComplete;

        private bool rosePlaced;
        private bool beaconPlaced;
        private bool witherSummoned;
        private bool witherKilled;
        private int witherSkeletonsKilled;

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

            //check wither rose status
            this.rosePlaced = progress.WasUsed(WitherRose);
            this.beaconPlaced = progress.WasUsed(Beacon);
            this.witherSummoned = progress.AdvancementCompleted(SummonWither);
            this.witherKilled = progress.CriterionCompleted(MonsterHunter, Wither);

            this.witherSkeletonsKilled = progress.TimesKilled(WitherSkeleton);

            if (Tracker.Category is AllBlocks)
            {
                this.Partial = !this.rosePlaced && !this.beaconPlaced;
                this.CompletionOverride = this.EstimatedObtained >= this.Required
                    || this.witherSummoned || this.witherKilled || this.rosePlaced || this.beaconPlaced;
            }
            else
            {
                this.Partial = !this.witherKilled;
                this.CompletionOverride = this.EstimatedObtained >= this.Required
                    || this.witherSummoned || this.fullBeaconComplete || this.witherKilled;
            }
        }

        protected override void ClearAdvancedState()
        {
            this.EstimatedObtained = 0;
            this.rosePlaced = false;
            this.beaconPlaced = false;
            this.witherKilled = false;
            this.witherSkeletonsKilled = 0;
        }

        protected override string GetShortStatus()
        {
            if (this.fullBeaconComplete)
                return "Done";

            if (this.witherKilled)
                return "Done";

            return $"{this.EstimatedObtained}\0/\0{this.Required}";
        }
            

        protected override string GetLongStatus()
        {
            if (Tracker.Category is AllBlocks)
            {
                if (this.beaconPlaced && this.rosePlaced)
                    return "Beacon+Rose\nPlaced";
            }

            if (this.witherKilled)
                return "Wither\0Has\nBeen\0Killed";

            if (this.witherSummoned)
                return "Wither\nSummoned";

            if (this.EstimatedObtained >= this.Required)
                return $"{this.EstimatedObtained}\0/\0{this.Required}\nKilled:\0{this.witherSkeletonsKilled}";

            return $"Skulls\n{this.EstimatedObtained}\0/\0{this.Required}";
        }

        protected override string GetCurrentIcon()
        {
            if (Tracker.Category is AllBlocks && this.beaconPlaced && this.rosePlaced)
                return "skull_and_beacon";

            return this.witherSummoned || this.witherKilled
                ? "wither_mob"
                : "get_wither_skull";
        }
    }
}
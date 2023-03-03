using System;
using System.Xml;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class PrismarineBlocks : ComplexObjective
    {
        public const string HdwghId = "minecraft:nether/all_effects"; 
        public const string Conduit = "minecraft:conduit";
        public const string Normal = "minecraft:prismarine";
        public const string Dark = "minecraft:dark_prismarine";
        public const string Bricks = "minecraft:prismarine_bricks";

        public const int Required = 16;

        private int total;
        private bool hdwghComplete; 
        private bool conduitPlaced;

        private bool HasEnoughForConduit => this.total >= Required;

        public PrismarineBlocks() : base() 
        {
            this.Icon = "all_prismarine_blocks";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.hdwghComplete = progress.AdvancementCompleted(HdwghId);

            int normal = progress.TimesPickedUp(Normal)
                + progress.TimesCrafted(Normal)
                - progress.TimesDropped(Normal)
                - progress.TimesUsed(Normal);
            int dark = progress.TimesPickedUp(Dark)
                + progress.TimesCrafted(Dark)
                - progress.TimesDropped(Dark)
                - progress.TimesUsed(Dark);
            int bricks = progress.TimesPickedUp(Bricks)
                + progress.TimesCrafted(Bricks)
                - progress.TimesDropped(Bricks)
                - progress.TimesUsed(Bricks);

            this.conduitPlaced = progress.TimesUsed(Conduit) > 0;

            this.total = Math.Max(normal + dark + bricks, 0);
            this.CompletionOverride = this.hdwghComplete || this.HasEnoughForConduit || this.conduitPlaced;
        }

        protected override string GetCurrentIcon() => "all_prismarine_blocks";

        protected override void ClearAdvancedState()
        {
            this.total = 0;
        }

        protected override string GetShortStatus()
        {
            if (this.hdwghComplete || this.conduitPlaced)
                return string.Empty;

            if (this.ManuallyChecked)
                return "Collected";

            return $"{this.total}\0/\0{Required}";
        }

        protected override string GetLongStatus()
        {
            if (this.hdwghComplete)
                return $"HDWGH\nCompleted";
            return $"Prismarine\n{this.total}\0/\0{Required}";
        }
    }
}

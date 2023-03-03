using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class Cauldrons : Pickup
    {
        public const string ItemId = "minecraft:cauldron";
        public const string LightAsARabbit = "minecraft:adventure/walk_on_powder_snow_with_leather_boots";

        private bool advancementComplete;
        private int placed;
        public Cauldrons() : base(ItemId)
        {
        }

        public override int Required => int.MaxValue;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.placed = progress.TimesUsed(ItemId);
            this.advancementComplete = progress.AdvancementCompleted(LightAsARabbit);
            base.UpdateAdvancedState(progress);
            this.CompletionOverride |= this.advancementComplete;
        }

        protected override void ClearAdvancedState()
        {
            base.ClearAdvancedState();
            this.advancementComplete = false;
            this.placed = 0;
        }

        protected override string GetShortStatus() =>
            this.advancementComplete ? "Done" : $"Placed:\0{this.placed}";
                
        protected override string GetLongStatus() =>
            this.advancementComplete ? "LaaR\nComplete" : $"Cauldrons\nPlaced:\0{this.placed}";

        protected override string GetCurrentIcon() => "cauldron";
    }
}

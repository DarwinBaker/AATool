using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class Pufferfish : Pickup
    {
        public const string HwdwghId = "minecraft:nether/all_effects";
        public const string BalancedDietId = "minecraft:husbandry/balanced_diet";
        public const string ItemId = "minecraft:pufferfish";

        private bool advancementsComplete;

        public Pufferfish() : base(ItemId)
        {
        }

        public override int Required => 2;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.advancementsComplete = progress.AdvancementCompleted(HwdwghId)
                && progress.AdvancementCompleted(BalancedDietId);
            base.UpdateAdvancedState(progress);
        }

        protected override void ClearAdvancedState()
        {
            base.ClearAdvancedState();
            this.advancementsComplete = false;
        }

        protected override string GetShortStatus() =>
            this.advancementsComplete ? "Done" : base.GetShortStatus();
                
        protected override string GetLongStatus() =>
            this.advancementsComplete ? "HDWGH\nComplete" : $"Pufferfish:\n{this.Obtained}\0/\0{this.Required}";

        protected override string GetCurrentIcon() => "pufferfish";
    }
}

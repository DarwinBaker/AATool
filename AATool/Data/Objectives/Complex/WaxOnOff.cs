using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class WaxOnOff : ComplexObjective
    {
        private const string WaxOn = "minecraft:husbandry/wax_on";
        private const string WaxOff = "minecraft:husbandry/wax_off";

        bool waxOnComplete;
        bool waxOffComplete;

        public WaxOnOff()
        {
            this.Icon = "wax_on";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.waxOnComplete = progress.AdvancementCompleted(WaxOn);
            this.waxOffComplete = progress.AdvancementCompleted(WaxOff);
            this.CompletionOverride = this.waxOnComplete && this.waxOffComplete;
        }

        protected override void ClearAdvancedState()
        {
            this.waxOnComplete = false;
            this.waxOffComplete = false;
        }

        protected override string GetLongStatus() => this.GetShortStatus();

        protected override string GetShortStatus()
        {
            if (this.waxOnComplete && !this.waxOffComplete)
                return "Wax Off";

            if (this.waxOffComplete && !this.waxOnComplete)
                return "Wax On";

            return "Wax On+Off";
        }

        protected override string GetCurrentIcon()
            => this.waxOnComplete && !this.waxOffComplete ? "wax_off" : "wax_on";
    }
}

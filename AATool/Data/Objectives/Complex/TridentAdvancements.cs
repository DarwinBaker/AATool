using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class TridentAdvancements : ComplexObjective
    {
        private const string ATJ = "minecraft:adventure/throw_trident";
        private const string VVF = "minecraft:adventure/very_very_frightening";

        bool atjComplete;
        bool vvfComplete;

        public TridentAdvancements()
        {
            this.Icon = "trident";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.atjComplete = progress.AdvancementCompleted(ATJ);
            this.vvfComplete = progress.AdvancementCompleted(VVF);
            this.CompletionOverride = this.atjComplete && this.vvfComplete;
        }

        protected override void ClearAdvancedState()
        {
            this.atjComplete = false;
            this.vvfComplete = false;
        }

        protected override string GetLongStatus() => this.GetShortStatus();

        protected override string GetShortStatus()
        {
            if (this.atjComplete && !this.vvfComplete)
                return "VVF";

            if (this.vvfComplete && !this.atjComplete)
                return "ATJ";

            return "ATJ + VVF";
        }
    }
}

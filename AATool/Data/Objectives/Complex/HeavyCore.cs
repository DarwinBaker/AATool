using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class HeavyCore : ComplexObjective
    {
        public const string HeavyCoreId = "minecraft:heavy_core";
        public const string MaceId = "minecraft:mace";
        public const string OminousTrialKeyId = "minecraft:ominous_trial_key";
        public const string OverOverkillAdvancement = "minecraft:adventure/overoverkill";

        public bool ObtainedHeavyCore { get; private set; }
        public bool MaceCrafted { get; private set; }
        public bool OverOverkillComplete { get; private set; }
        public int OminousVaultsOpened { get; private set; }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.ObtainedHeavyCore = progress.WasPickedUp(HeavyCoreId);

            this.MaceCrafted = progress.WasCrafted(MaceId);

            this.OverOverkillComplete = progress.AdvancementCompleted(OverOverkillAdvancement);

            this.OminousVaultsOpened = progress.TimesUsed(OminousTrialKeyId);

            this.Partial = !this.OverOverkillComplete;
            this.CompletionOverride = this.OverOverkillComplete || this.MaceCrafted || this.ObtainedHeavyCore;
        }

        protected override void ClearAdvancedState()
        {
            this.ObtainedHeavyCore = false;
            this.MaceCrafted = false;
            this.OverOverkillComplete = false;
            this.OminousVaultsOpened = 0;
        }

        protected override string GetShortStatus()
        {
            if (this.OverOverkillComplete)
                return "Done";

            if (this.MaceCrafted)
                return "Mace\0Crafted";

            if (this.ObtainedHeavyCore)
                return "Heavy\0Core";

            return "Heavy\0Core";
        }

        protected override string GetLongStatus()
        {
            if (this.OverOverkillComplete)
                return "Overkill\nComplete";

            if (this.MaceCrafted)
                return "Mace\nCrafted";

            if (this.ObtainedHeavyCore)
                return $"Obtained\nVaults:\0{this.OminousVaultsOpened}";

            return "Obtain\nHeavy\0Core";
        }

        protected override string GetCurrentIcon()
        {
            if (this.OverOverkillComplete || this.MaceCrafted)
                return "overoverkill";

            return "heavy_core";
        }
    }
}
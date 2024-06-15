using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class HeavyCore : ComplexObjective
    {
        public const string MaceRecipe = "minecraft:recipes/combat/mace";
        public const string MaceId = "minecraft:mace";
        public const string BreezeRodId = "minecraft:breeze_rod";
        public const string OverOverkillAdvancement = "minecraft:adventure/overoverkill";

        public bool ObtainedHeavyCore { get; private set; }
        public bool ObtainedBreezeRod { get; private set; }
        public bool MaceCrafted { get; private set; }
        public bool OverOverkillComplete { get; private set; }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.ObtainedHeavyCore = progress.ObtainedHeavyCore;

            this.ObtainedBreezeRod = progress.WasPickedUp(BreezeRodId);

            this.MaceCrafted = progress.WasCrafted(MaceId);

            this.OverOverkillComplete = progress.AdvancementCompleted(OverOverkillAdvancement);

            this.Partial = !this.OverOverkillComplete;
            this.CompletionOverride = this.OverOverkillComplete || this.MaceCrafted || this.ObtainedHeavyCore;
        }

        protected override void ClearAdvancedState()
        {
            this.ObtainedHeavyCore = false;
            this.ObtainedBreezeRod = false;
            this.MaceCrafted = false;
            this.OverOverkillComplete = false;
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
                return "Obtained\nHeavy\0Core";

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
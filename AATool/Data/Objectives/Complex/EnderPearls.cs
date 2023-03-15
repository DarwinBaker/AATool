using System;
using System.Xml;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class EnderPearls : ComplexObjective
    {
        public const string PearlId = "minecraft:ender_pearl";
        public const string EyeId = "minecraft:ender_eye";

        int estimate;

        public EnderPearls() : base() 
        {
            this.Icon = "ender_pearl";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.estimate = progress.TimesPickedUp(PearlId)
                - progress.TimesDropped(PearlId)
                - progress.TimesUsed(PearlId)
                - progress.TimesCrafted(EyeId);
            this.estimate = Math.Max(0, this.estimate);
            this.CompletionOverride = this.estimate > 0;
        }

        protected override void ClearAdvancedState()
        {
            this.estimate = 0;
        }

        protected override string GetShortStatus()
        {
            return $"{this.estimate}\0Pearls";
        }

        protected override string GetLongStatus()
        {
            return $"{this.estimate}\0Pearls";
        }
    }
}

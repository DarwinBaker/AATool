using AATool.Configuration;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class Netherite : AncientDebris
    {
        /*
        protected override string GetShortStatus()
        {
            if (this.CompletedSeriousDedication && this.CompletedCoverMeInDebris)
                return "Done";
            return "Smithing";
        }
        */

        protected override string GetCurrentIcon()
        {
            if (this.CompletedSeriousDedication && this.CompletedCoverMeInDebris)
                return "smithing_both";
            if (this.CompletedSeriousDedication)
                return "smithing_hoe";
            if (this.CompletedCoverMeInDebris)
                return "smithing_armor";
            return "smithing_none";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        { 
            base.UpdateAdvancedState(progress);
            this.CompletionOverride |= this.CompletedSeriousDedication && this.CompletedCoverMeInDebris;
            this.CanBeManuallyChecked = false;
        }
    }
}

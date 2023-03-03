using AATool.Configuration;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class Tnt : AncientDebris
    {
        protected override string GetShortStatus()
        {
            if (this.AllNetheriteAdvancementsComplete)
                return "Done";
            return $"TNT: {this.EstimatedTnt}";
        }
                
        protected override string GetLongStatus() =>
            this.AllNetheriteAdvancementsComplete ? "Done\0Mining\nDebris" : $"TNT: {this.EstimatedTnt}";

        protected override string GetCurrentIcon() => "tnt_block";

        protected override void UpdateAdvancedState(ProgressState progress)
        { 
            base.UpdateAdvancedState(progress);
            this.CanBeManuallyChecked = false;
        }
    }
}

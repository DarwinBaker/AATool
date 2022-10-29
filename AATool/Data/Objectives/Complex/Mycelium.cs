using System.Xml;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class Mycelium : ComplexObjective
    {
        public const string BlockId = "minecraft:mycelium";

        public Mycelium() : base() 
        {
            this.Name = "Mycelium";
            this.Icon = "mycelium";
        }

        private bool obtained;
        private bool placed;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.obtained = progress.WasPickedUp(BlockId);
            this.placed = progress.WasUsed(BlockId);
            this.CompletionOverride = this.obtained || this.placed;
        }

        protected override void ClearAdvancedState()
        {
            this.obtained = false;
            this.placed = false;
        }

        protected override string GetShortStatus()
        {
            if (this.placed)
                return "Placed";
            else if (this.obtained)
                return "Obtained";
            else
                return "0";
        }

        protected override string GetLongStatus()
        {
            if (this.placed)
                return "Mycelium Placed";
            else if (this.obtained)
                return "Mycelium Obtained";
            else
                return "Obtain Mycelium";
        }
    }
}

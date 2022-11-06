using System.Xml;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class RedSand : ComplexObjective
    {
        public const string BlockId = "minecraft:red_sand";

        private bool obtained;
        private bool placed;

        public RedSand() : base() 
        {
            this.Icon = "red_sand";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.obtained = progress.WasPickedUp(BlockId);
            this.placed = progress.WasUsed(BlockId);
            this.CompletionOverride = this.obtained || this.placed;
        }

        protected override void ClearAdvancedState()
        {
        }

        protected override string GetShortStatus() => "Red\0Sand";

        protected override string GetLongStatus()
        {
            if (this.placed)
                return "Red\0Sand\nPlaced";

            if (this.obtained)
                return "Obtained\nRed\0Sand";

            return "Obtain\nRed\0Sand";
        }
    }
}

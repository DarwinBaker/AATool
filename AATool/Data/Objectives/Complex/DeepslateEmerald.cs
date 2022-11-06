using System.Xml;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class DeepslateEmerald : ComplexObjective
    {
        public const string BlockId = "minecraft:deepslate_emerald_ore";

        public DeepslateEmerald() : base() 
        {
            this.Icon = "deepslate_emerald_ore";
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

        protected override string GetShortStatus() => "Deepslate Emerald";

        protected override string GetLongStatus()
        {
            if (this.placed)
                return "DS\0Emerald\nPlaced";

            if (this.obtained)
                return "DS\0Emerald\nObtained";

            return "Deepslate\nEmerald";
        }
    }
}

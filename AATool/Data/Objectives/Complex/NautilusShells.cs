using System;
using System.Diagnostics;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class NautilusShells : ComplexPickupObjective
    {
        private const string HDWGH = "minecraft:nether/all_effects";
        private const string Conduit = "minecraft:conduit";

        public override int Required => 8;

        private bool conduitCrafted;
        private bool conduitPlaced;
        private bool hdwghComplete;

        public NautilusShells() : base("minecraft:nautilus_shell")
        { 
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            base.UpdateAdvancedState(progress);

            this.hdwghComplete = progress.AdvancementCompleted(HDWGH);
            this.conduitCrafted = progress.WasCrafted(Conduit);
            this.conduitPlaced = progress.WasUsed(Conduit);

            this.CompletionOverride |= this.conduitCrafted || this.conduitPlaced;

            if (Tracker.Category is not AllBlocks)
                this.CompletionOverride |= this.hdwghComplete;
        }

        protected override void ClearAdvancedState()
        {
            base.ClearAdvancedState();
            this.conduitCrafted = false;
            this.conduitPlaced = false;
            this.hdwghComplete = false;
        }

        protected override string GetLongStatus()
        {
            if (Tracker.Category is not AllBlocks && this.hdwghComplete)
                return "HDWGH Complete";

            if (this.conduitPlaced)
                return "Conduit Placed";

            if (this.conduitCrafted)
                return "Conduit Crafted";

            return $"Shells\n{this.Obtained} / {this.Required}";
        }

        protected override string GetCurrentIcon()
        {
            if (Tracker.Category is not AllBlocks)
                return "nautilus_shell";

            return this.conduitCrafted || this.conduitPlaced
                ? "conduit"
                : "nautilus_shell";
        }
    }
}

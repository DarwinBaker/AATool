using System;
using System.Diagnostics;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class NautilusShells : Pickup
    {
        private const string HDWGH = "minecraft:nether/all_effects";
        private const string Conduit = "minecraft:conduit";

        public override int Required => 8;

        private bool conduitCrafted;
        private bool conduitPlaced;
        private bool hdwghComplete;

        public NautilusShells() : base("minecraft:nautilus_shell")
        {
            this.Name = this.GetType().Name;
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

        protected override string GetShortStatus()
        {
            if (this.hdwghComplete)
                return "Done";

            if (this.conduitPlaced)
                return "Ready";

            return $"{this.Obtained}\0/\0{this.Required}";
        }

        protected override string GetLongStatus()
        {
            if (Tracker.Category is not AllBlocks && this.hdwghComplete)
                return "HDWGH\nComplete";

            if (this.conduitPlaced)
                return "Conduit\nPlaced";

            if (this.conduitCrafted)
                return "Conduit\nCrafted";

            return $"Shells\n{this.Obtained}\0/\0{this.Required}";
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

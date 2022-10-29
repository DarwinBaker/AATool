using System;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class NautilusShells : ComplexObjective
    {
        public const string ItemId = "minecraft:nautilus_shell";
        private const string HDWGH = "minecraft:nether/all_effects";
        private const string Conduit = "minecraft:conduit";

        public const int Required = 8;

        public int EstimatedObtained { get; private set; }

        private bool conduitCrafted;
        private bool conduitPlaced;
        private bool hdwghComplete;

        public NautilusShells() : base() 
        {
            this.Name = "NautilusShells";
            this.Icon = "nautilus_shell";
            //if (Tracker.Category is AllBlocks)
            //    this.Icon = "shell_and_conduit";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.EstimatedObtained = progress.TimesPickedUp(ItemId)
                - progress.TimesDropped(ItemId);
            this.EstimatedObtained = Math.Max(0, this.EstimatedObtained);

            this.hdwghComplete = progress.AdvancementCompleted(HDWGH);
            this.conduitCrafted = progress.WasCrafted(Conduit);
            this.conduitPlaced = progress.WasUsed(Conduit);

            this.CompletionOverride = this.conduitCrafted || this.conduitPlaced
                || this.EstimatedObtained >= Required;

            if (Tracker.Category is not AllBlocks)
                this.CompletionOverride |= this.hdwghComplete;
        }

        protected override void ClearAdvancedState()
        {
            this.EstimatedObtained = 0;
            this.conduitCrafted = false;
            this.conduitPlaced = false;
            this.hdwghComplete = false;
        }

        protected override string GetShortStatus() => $"{this.EstimatedObtained} / {Required}";

        protected override string GetLongStatus()
        {
            if (Tracker.Category is not AllBlocks && this.hdwghComplete)
                return "HDWGH Complete";
            if (this.conduitPlaced)
                return "Conduit Placed";
            if (this.conduitCrafted)
                return "Conduit Crafted";

            return $"Shells\n{this.EstimatedObtained} / {Required}";
        }
    }
}

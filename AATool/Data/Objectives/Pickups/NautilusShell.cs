using System.Xml;
using AATool.Data.Categories;

namespace AATool.Data.Objectives.Pickups
{
    class NautilusShell : Pickup
    {
        public const string ItemId = "minecraft:nautilus_shell";
        private const string HDWGH = "minecraft:nether/all_effects";
        private const string Conduit = "minecraft:conduit";

        private bool placedConduit;

        public NautilusShell(XmlNode node) : base(node) 
        {
            //if (Tracker.Category is AllBlocks)
            //    this.Icon = "shell_and_conduit";
        }

        protected override void HandleCompletionOverrides()
        {
            this.placedConduit = Tracker.TryGetBlock(Conduit, out Block conduit)
                && conduit.IsComplete();

            if (Tracker.Category is AllBlocks)
            {
                this.CompletionOverride = this.placedConduit;
            }
            else
            {
                //check if hdwgh complete
                Tracker.TryGetAdvancement(HDWGH, out Advancement hdwgh);
                this.CompletionOverride = hdwgh?.IsComplete() is true;
            }
        }

        protected override void UpdateLongStatus()
        {
            //show if conduit has been placed or if hdwgh is complete
            if (this.CompletionOverride)
                this.FullStatus = Tracker.Category is AllBlocks ? "Conduit Placed" : "HDWGH Complete";
            else if (this.placedConduit)
                this.FullStatus = "Conduit Placed";
            else
                base.UpdateLongStatus();
        }
    }
}

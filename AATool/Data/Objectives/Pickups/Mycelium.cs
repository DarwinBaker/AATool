using System.Xml;

namespace AATool.Data.Objectives.Pickups
{
    class Mycelium : Pickup
    {
        public const string BlockId = "minecraft:mycelium";

        public Mycelium(XmlNode node) : base(node) { }

        private bool placed;

        protected override void HandleCompletionOverrides()
        {
            //ignore count if full beacon has been constructed
            if (Tracker.TryGetBlock(BlockId, out Block mycelium))
            {
                this.placed = mycelium.HasBeenPlaced;
                this.CompletionOverride = this.placed;
            }
        }

        protected override void UpdateLongStatus()
        {
            if (this.placed)
                this.FullStatus = "Mycelium Placed";
            else 
                this.FullStatus = this.PickedUp > 0 ? "Mycelium Obtained" : "Obtain Mycelium";
        }
    }
}

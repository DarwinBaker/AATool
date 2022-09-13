using System.Xml;

namespace AATool.Data.Objectives.Pickups
{
    class DeepslateEmerald : Pickup
    {
        public const string BlockId = "minecraft:deepslate_emerald_ore";

        public DeepslateEmerald(XmlNode node) : base(node) { }

        private bool placed;

        protected override void HandleCompletionOverrides()
        {
            //ignore count if full beacon has been constructed
            if (Tracker.TryGetBlock(BlockId, out Block deepslateEmerald))
            {
                this.placed = deepslateEmerald.HasBeenPlaced;
                this.CompletionOverride = this.placed;
            }
        }

        protected override void UpdateLongStatus()
        {
            if (this.placed)
                this.FullStatus = "DS Emerald Placed";
            else 
                this.FullStatus = this.PickedUp > 0 ? "DS Emerald Obtained" : "Deepslate Emerald";
        }
    }
}

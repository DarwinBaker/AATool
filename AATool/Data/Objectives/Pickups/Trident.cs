using System.Xml;

namespace AATool.Data.Objectives.Pickups
{
    class Trident : Pickup
    {
        public const string ItemId = "minecraft:trident";
        private const string VVF = "minecraft:adventure/very_very_frightening";
        private const string Surge = "minecraft:adventure/lightning_rod_with_villager_no_fire";

        private bool vvfDone;
        private bool surgeDone;
        private bool surgeAdded;

        public Trident(XmlNode node) : base(node) { }

        protected override void HandleCompletionOverrides()
        {
            //get advancements requiring thunder
            this.vvfDone = Tracker.TryGetAdvancement(VVF, out Advancement vvf)
                && vvf.CompletedByAnyone();

            //ignore surge prot if it's not in the game yet (pre-1.17)
            this.surgeAdded = Tracker.TryGetAdvancement(Surge, out Advancement surge);
            this.surgeDone = !this.surgeAdded || surge.CompletedByAnyone();

            this.CompletionOverride = this.vvfDone && this.surgeDone;
        }

        protected override void UpdateLongStatus()
        {
            //override status with current state of thunder
            if (this.CompletionOverride)
            {
                this.FullStatus = "Done With Thunder";
            }
            else if (this.vvfDone == this.surgeDone || !this.surgeAdded)
            {
                //not done with either, see if we still need trident too
                this.FullStatus = this.PickedUp > 0
                    ? "Awaiting Thunder"
                    : "Obtain Trident";
            }
            else
            {
                //only one of the two thunder-related advancements are complete
                this.FullStatus = this.vvfDone
                    ? "Surge Prot Incomplete"
                    : "VVF Incomplete";
            }
        }
    }
}

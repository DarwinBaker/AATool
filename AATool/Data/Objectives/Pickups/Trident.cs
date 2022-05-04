using System.Xml;
using AATool.Data.Categories;

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

        private bool zombieHead;
        private bool creeperHead;
        private bool skeletonSkull;
        private int headsObtained;

        public Trident(XmlNode node) : base(node) { }

        protected override void HandleCompletionOverrides()
        {
            if (Tracker.Category is AllBlocks)
            {
                this.headsObtained = 0;

                //check if all heads requiring charged creepers have been obtained
                Tracker.TryGetBlock("minecraft:zombie_head", out Block zombie);
                this.zombieHead = zombie?.CompletedByAnyone() is true;
                if (this.zombieHead) 
                    this.headsObtained++;

                Tracker.TryGetBlock("minecraft:creeper_head", out Block creeper);
                this.creeperHead = creeper?.CompletedByAnyone() is true;
                if (this.creeperHead)
                    this.headsObtained++;

                Tracker.TryGetBlock("minecraft:skeleton_skull", out Block skeleton);
                this.skeletonSkull = skeleton?.CompletedByAnyone() is true;
                if (this.skeletonSkull)
                    this.headsObtained++;

                this.CompletionOverride = this.zombieHead && this.creeperHead && this.skeletonSkull;
            }
            else
            {
                //get advancements requiring thunder
                this.vvfDone = Tracker.TryGetAdvancement(VVF, out Advancement vvf)
                    && vvf.IsComplete();

                //ignore surge prot if it's not in the game yet (pre-1.17)
                this.surgeAdded = Tracker.TryGetAdvancement(Surge, out Advancement surge);
                this.surgeDone = !this.surgeAdded || surge.IsComplete();

                this.CompletionOverride = this.vvfDone && this.surgeDone;
            }
        }

        protected override void UpdateLongStatus()
        {
            //override status with current state of thunder
            this.FullStatus = Tracker.Category is AllBlocks 
                ? this.BlocksStatus() 
                : this.AdvancementsStatus();
        }

        private string AdvancementsStatus()
        {
            string status;
            if (this.CompletionOverride)
            {
                status = "Done With Thunder";
            }
            else if (this.vvfDone == this.surgeDone || !this.surgeAdded)
            {
                //not done with either, see if we still need trident too
                status = this.PickedUp > 0
                    ? "Awaiting Thunder"
                    : "Obtain Trident";
            }
            else
            {
                //only one of the two thunder-related advancements are complete
                status = this.vvfDone
                    ? "Surge Prot Incomplete"
                    : "VVF Incomplete";
            }
            return status;
        }

        private string BlocksStatus()
        {
            string status;
            if (this.CompletionOverride)
            {
                this.Icon = "trident_and_heads";
                status = "Done With Thunder";
            }
            else
            {
                this.Icon = "trident";
                status = this.PickedUp > 0
                    ? "Awaiting Thunder"
                    : "Obtain Trident";
            }
            return status;
        }
    }
}

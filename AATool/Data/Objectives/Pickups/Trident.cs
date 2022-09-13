using System;
using System.Xml;
using AATool.Data.Categories;

namespace AATool.Data.Objectives.Pickups
{
    class Trident : Pickup
    {
        public const string ItemId = "minecraft:trident";
        private const string VVF = "minecraft:adventure/very_very_frightening";
        private const string Surge = "minecraft:adventure/lightning_rod_with_villager_no_fire";

        private static readonly Version AncientCitySkeletonSkulls = new ("1.19");

        private bool vvfDone;
        private bool surgeDone;
        private bool surgeAdded;

        private bool zombieHead;
        private bool creeperHead;
        private bool skeletonSkull;

        public Trident(XmlNode node) : base(node) { }

        protected override void HandleCompletionOverrides()
        {
            if (Tracker.Category is AllBlocks)
            {
                this.zombieHead = false;
                this.creeperHead = false;
                this.skeletonSkull = false;

                //check if all heads requiring charged creepers have been obtained
                if (Tracker.TryGetBlock("minecraft:zombie_head", out Block zombie))
                    this.zombieHead = zombie.CompletedByAnyone() || zombie.PickedUpByAnyone();

                if (Tracker.TryGetBlock("minecraft:creeper_head", out Block creeper))
                    this.creeperHead = creeper.CompletedByAnyone() || creeper.PickedUpByAnyone();

                if (Tracker.TryGetBlock("minecraft:skeleton_skull", out Block skeleton))
                    this.skeletonSkull = skeleton.CompletedByAnyone() || skeleton.PickedUpByAnyone();

                if (Version.TryParse(Tracker.Category.CurrentVersion, out Version current) && current >= AncientCitySkeletonSkulls)
                {
                    //post-1.19, skeleton skulls are available in ancient city and no longer require thunder
                    this.CompletionOverride = this.zombieHead && this.creeperHead;
                }
                else
                {
                    this.CompletionOverride = this.zombieHead && this.creeperHead && this.skeletonSkull;
                }
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
                ? this.GetStatusAB() 
                : this.GetStatusAA();
        }

        private string GetStatusAA()
        {
            string status;
            if (this.CompletionOverride)
            {
                this.Icon = "enchanted_trident";
                status = "Done With Thunder";
            }
            else
            {
                this.Icon = "trident";
                if (this.vvfDone == this.surgeDone || !this.surgeAdded)
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
            }
            return status;
        }

        private string GetStatusAB()
        {
            string status;
            if (this.CompletionOverride)
            {
                this.Icon = "enchanted_trident";
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

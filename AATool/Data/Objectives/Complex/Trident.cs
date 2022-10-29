using System;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Complex
{
    public class Trident : ComplexObjective
    {
        public const string ItemId = "minecraft:trident";
        private const string VVF = "minecraft:adventure/very_very_frightening";
        private const string Surge = "minecraft:adventure/lightning_rod_with_villager_no_fire";

        private static readonly Version AncientCitySkeletonSkulls = new ("1.19");
        private static readonly Version SurgeProtectorAdded = new ("1.17");

        private bool obtained;

        private bool vvfDone;
        private bool surgeDone;
        private bool ignoreSurge;

        private bool zombieHead;
        private bool creeperHead;
        private bool skeletonSkull;

        public Trident() : base() 
        {
            this.Name = "Trident";
            this.Icon = "trident";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.obtained = progress.WasPickedUp(ItemId);

            if (Tracker.Category is AllBlocks)
            {
                this.zombieHead = progress.WasUsed("minecraft:zombie_head")
                    || progress.WasPickedUp("minecraft:zombie_head");
                this.creeperHead = progress.WasUsed("minecraft:creeper_head")
                    || progress.WasPickedUp("minecraft:creeper_head");
                this.skeletonSkull = progress.WasUsed("minecraft:skeleton_skull")
                    || progress.WasPickedUp("minecraft:skeleton_skull");

                this.CompletionOverride = this.zombieHead && this.creeperHead;
                //post-1.19, skeleton skulls are available in ancient city and no longer require thunder
                if (Version.TryParse(Tracker.Category.CurrentVersion, out Version current) && current < AncientCitySkeletonSkulls)
                    this.CompletionOverride &= this.skeletonSkull;
            }
            else
            {
                //get advancements requiring thunder
                this.vvfDone = progress.AdvancementCompleted(VVF);
                this.surgeDone = progress.AdvancementCompleted(Surge);
                this.ignoreSurge = Version.TryParse(Tracker.Category.CurrentVersion, out Version current) && current < SurgeProtectorAdded;

                if (this.ignoreSurge)
                {
                    //ignore surge protector, not in the game yet (pre-1.17)
                    this.CompletionOverride = this.obtained || this.vvfDone;
                }
                else
                {
                    this.CompletionOverride = this.obtained || this.vvfDone && this.surgeDone;
                }
            }
        }

        protected override void ClearAdvancedState()
        {
            this.obtained = false;

            this.vvfDone = false;
            this.surgeDone = false;

            this.zombieHead = false;
            this.creeperHead = false;
            this.skeletonSkull = false;
        }

        protected override string GetLongStatus()
        {
            //override status with current state of thunder
            return Tracker.Category is AllBlocks 
                ? this.GetStatusAB() 
                : this.GetStatusAA();
        }

        private string GetStatusAA()
        {
            string status;
            if (this.vvfDone && (this.surgeDone || this.ignoreSurge))
            {
                this.Icon = "enchanted_trident";
                status = "Done\0With\nThunder";
            }
            else
            {
                this.Icon = "trident";
                if (this.vvfDone == this.surgeDone || this.ignoreSurge)
                {
                    //not done with either, see if we still need trident too
                    status = this.obtained
                        ? "Awaiting\nThunder"
                        : "Obtain\nTrident";
                }
                else
                {
                    //only one of the two thunder-related advancements are complete
                    status = this.vvfDone
                        ? "Surge\0Prot\nIncomplete"
                        : "VVF\nIncomplete";
                }
            }
            return status;
        }

        protected override string GetShortStatus() => this.obtained ? "Obtained" : "0";

        private string GetStatusAB()
        {
            string status;
            if (this.CompletionOverride)
            {
                this.Icon = "enchanted_trident";
                status = "Done\0With\nThunder";
            }
            else
            {
                this.Icon = "trident";
                status = this.obtained
                    ? "Awaiting\nThunder"
                    : "Obtain\nTrident";
            }
            return status;
        }
    }
}

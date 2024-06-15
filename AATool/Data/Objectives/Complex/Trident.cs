using System;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Complex
{
    public class Trident : ComplexObjective
    {
        private const string VVF = "minecraft:adventure/very_very_frightening";
        private const string Surge = "minecraft:adventure/lightning_rod_with_villager_no_fire";

        private static readonly Version PiglinHeadAdded = new ("1.20");
        private static readonly Version AncientCitySkeletonSkulls = new ("1.19");
        private static readonly Version SurgeProtectorAdded = new ("1.17");
        private static readonly Version TridentFoundInVaults = new ("1.21");

        private bool obtained;

        private bool vvfDone;
        private bool surgeDone;
        private bool ignoreSurge;

        private bool piglinHead;
        private bool zombieHead;
        private bool creeperHead;
        private bool skeletonSkull;

        private bool doneWithHeads;

        public bool EnchantedForegroundLayer => this.doneWithHeads;

        public Trident()
        { 
            this.Id = "minecraft:trident";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.obtained = progress.WasPickedUp(this.Id);

            Version.TryParse(Tracker.Category.CurrentVersion, out Version current);
            current ??= new Version();

            this.CanBeManuallyChecked = current >= TridentFoundInVaults && !this.obtained;

            this.CompletionOverride = this.obtained || (this.ManuallyChecked && this.CanBeManuallyChecked);

            if (Tracker.Category is AllBlocks)
            {
                //post-1.19, skeleton skulls are available in ancient city and no longer require thunder
                bool ancientCitiesExist = current >= AncientCitySkeletonSkulls;
                bool piglinHeadRequired = current >= PiglinHeadAdded;

                this.zombieHead = progress.WasUsed("minecraft:zombie_head")
                    || progress.WasPickedUp("minecraft:zombie_head");

                this.creeperHead = progress.WasUsed("minecraft:creeper_head")
                    || progress.WasPickedUp("minecraft:creeper_head");

                this.skeletonSkull = progress.WasUsed("minecraft:skeleton_skull")
                    || progress.WasPickedUp("minecraft:skeleton_skull") || ancientCitiesExist;

                this.piglinHead = progress.WasUsed("minecraft:piglin_head")
                    | progress.WasPickedUp("minecraft:piglin_head");

                this.doneWithHeads = this.zombieHead && this.creeperHead && this.skeletonSkull;
                if (piglinHeadRequired)
                    this.doneWithHeads &= this.piglinHead;

                this.Partial = !this.doneWithHeads;
                this.CompletionOverride |= this.doneWithHeads;
            }
            else
            {
                //get advancements requiring thunder
                this.vvfDone = progress.AdvancementCompleted(VVF);
                this.surgeDone = progress.AdvancementCompleted(Surge);
                this.ignoreSurge = current < SurgeProtectorAdded;

                if (this.ignoreSurge)
                {
                    //ignore surge protector, not in the game yet (pre-1.17)
                    this.CompletionOverride |= this.vvfDone;
                    this.Partial = !this.vvfDone;
                }
                else
                {
                    this.CompletionOverride |= this.vvfDone && this.surgeDone;
                    this.Partial = !(this.vvfDone && this.surgeDone);
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
            if (this.vvfDone && (this.surgeDone || this.ignoreSurge))
                return "Done\0With\nThunder";

            if (this.vvfDone == this.surgeDone || this.ignoreSurge)
            {
                //not done with either, see if we still need trident too
                return this.obtained || (this.ManuallyChecked && this.CanBeManuallyChecked)
                    ? "Awaiting\nThunder"
                    : "Obtain\nTrident";
            }

            //only one of the two thunder-related advancements are complete
            return this.vvfDone
                ? "Still\0Needs\nSurge\0Prot"
                : "Still\0Needs\nVVF";
        }

        protected override string GetShortStatus()
        {
            if (this.vvfDone && (this.surgeDone || this.ignoreSurge))
                return "Done";

            return this.obtained || (this.ManuallyChecked && this.CanBeManuallyChecked) 
                ? "Obtained" 
                : "Trident";
        }

        private string GetStatusAB()
        {
            if (this.doneWithHeads)
                return "Done\0With\nThunder";

            return this.obtained || (this.ManuallyChecked && this.CanBeManuallyChecked)
                ? "Awaiting\nThunder"
                : "Obtain\nTrident";
        }

        protected override string GetCurrentIcon()
        {
            if (Tracker.Category is AllBlocks)
            {
                bool piglinHead = Version.TryParse(Tracker.Category.CurrentVersion, out Version current)
                    && current >= PiglinHeadAdded;

                return this.doneWithHeads
                    ? (piglinHead ? "trident_and_heads_1.20" : "trident_and_heads")
                    : "trident";
            }

            if (this.vvfDone && !this.surgeDone && !this.ignoreSurge)
                return "lightning_rod";

            if (this.obtained)
                return "enchanted_trident";

            return "trident";
        }
    }
}

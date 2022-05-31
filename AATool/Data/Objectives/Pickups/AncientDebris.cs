using System;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Pickups
{
    class AncientDebris : Pickup
    {
        public const string ItemId = "minecraft:ancient_debris";
        private const string ObtainDebris = "minecraft:nether/obtain_ancient_debris";
        private const string UseLodestone = "minecraft:nether/use_lodestone";
        private const string NetheriteHoe = "minecraft:husbandry/obtain_netherite_hoe";
        private const string NetheriteArmor = "minecraft:nether/netherite_armor";

        public AncientDebris(XmlNode node) : base(node) 
        {
            if (Tracker.Category is AllBlocks)
            {
                this.Icon = "netherite_block";
                this.TargetCount = 37;
            }
        }
       
        private bool completedHiddenInTheDepths;
        private bool completedCountryLode;
        private bool completedSeriousDedication;
        private bool completedCoverMeInDebris;

        private bool allNetheriteAdvancementsComplete;

        protected override void HandleCompletionOverrides()
        {
            if (this.ManuallyChecked)
            {
                this.CompletionOverride = true;
                return;
            }

            //get netherite-related advancements
            Tracker.TryGetAdvancement(ObtainDebris, out Advancement hiddenInTheDepths);
            Tracker.TryGetAdvancement(UseLodestone, out Advancement countryLode);
            Tracker.TryGetAdvancement(NetheriteHoe, out Advancement seriousDedication);
            Tracker.TryGetAdvancement(NetheriteArmor, out Advancement coverMeInDebris);

            this.completedHiddenInTheDepths = hiddenInTheDepths?.IsComplete() is true;
            this.completedCountryLode = countryLode?.IsComplete() is true;
            this.completedSeriousDedication = seriousDedication?.IsComplete() is true;
            this.completedCoverMeInDebris = coverMeInDebris?.IsComplete() is true;

            this.allNetheriteAdvancementsComplete = this.completedHiddenInTheDepths
                && this.completedCountryLode
                && this.completedSeriousDedication
                && this.completedCoverMeInDebris;

            //ignore count if all netherite related advancements are done
            this.CompletionOverride = this.allNetheriteAdvancementsComplete;
        }

        public override void UpdateState(WorldState progress)
        {
            base.UpdateState(progress);
            this.CanBeManuallyChecked = !this.allNetheriteAdvancementsComplete;
        }

        protected override void UpdateLongStatus()
        {
            if (this.allNetheriteAdvancementsComplete)
            {
                this.FullStatus = "Done With Netherite";
            }
            else if (this.PickedUp >= this.TargetCount || this.ManuallyChecked)
            {
                this.FullStatus = "All Debris Collected";
            }
            else
            {
                int estimatedTNT = Math.Max(Tracker.State.TNTPickedUp - Tracker.State.TNTPlaced, 0);
                this.FullStatus = $"Debris: {this.GetTotal()}\nTNT: {estimatedTNT}";
            }
        }
    }
}

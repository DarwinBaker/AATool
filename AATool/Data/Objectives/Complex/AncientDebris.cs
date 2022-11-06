using System;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Complex
{
    class AncientDebris : ComplexObjective
    {
        public const string AncientDebrisId = "minecraft:ancient_debris";
        public const string NetheriteScrapId = "minecraft:netherite_scrap";
        public const string NetheriteIngotId = "minecraft:netherite_ingot";

        private const string NetheriteBlock = "minecraft:netherite_block";
        private const string ObtainDebris = "minecraft:nether/obtain_ancient_debris";
        private const string UseLodestone = "minecraft:nether/use_lodestone";
        private const string NetheriteHoe = "minecraft:husbandry/obtain_netherite_hoe";
        private const string NetheriteArmor = "minecraft:nether/netherite_armor";
        private const string Tnt = "minecraft:tnt";

        public const int Required = 37;

        public int EstimatedDebris { get; private set; }
        public int EstimatedTnt { get; private set; }

        private bool completedHiddenInTheDepths;
        private bool completedCountryLode;
        private bool completedSeriousDedication;
        private bool completedCoverMeInDebris;

        private bool allNetheriteAdvancementsComplete;
        
        private bool craftedNetheriteBlock;
        private bool placedNetheriteBlock;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.EstimatedDebris = progress.TimesPickedUp(AncientDebrisId)
                - Tracker.State.TimesDropped(AncientDebrisId);
            this.EstimatedDebris = Math.Max(0, this.EstimatedDebris);

            this.EstimatedTnt = progress.TimesPickedUp(Tnt)
                + Tracker.State.TimesCrafted(Tnt)
                - Tracker.State.TimesUsed(Tnt)
                - Tracker.State.TimesDropped(Tnt);
            this.EstimatedTnt = Math.Max(0, this.EstimatedTnt);

            if (Tracker.Category is AllBlocks)
            {
                //ignore count if netherite block has been placed
                this.craftedNetheriteBlock = progress.WasCrafted(NetheriteBlock);
                this.placedNetheriteBlock = progress.WasUsed(NetheriteBlock);
                this.CompletionOverride = this.placedNetheriteBlock;
            }
            else
            {
                //ignore count if all netherite related advancements are done
                this.completedHiddenInTheDepths = progress.AdvancementCompleted(ObtainDebris);
                this.completedCountryLode = progress.AdvancementCompleted(UseLodestone);
                this.completedSeriousDedication = progress.AdvancementCompleted(NetheriteHoe);
                this.completedCoverMeInDebris = progress.AdvancementCompleted(NetheriteArmor);

                this.allNetheriteAdvancementsComplete = this.completedHiddenInTheDepths
                    && this.completedCountryLode
                    && this.completedSeriousDedication
                    && this.completedCoverMeInDebris;

                this.CompletionOverride = this.allNetheriteAdvancementsComplete
                    || this.EstimatedDebris >= Required;
            }

            this.CanBeManuallyChecked = !this.CompletionOverride;
            if (this.ManuallyChecked)
                this.CompletionOverride = true;
        }

        protected override void ClearAdvancedState()
        {
            this.EstimatedDebris = 0;
            this.EstimatedTnt = 0;

            this.completedHiddenInTheDepths = false;
            this.completedCountryLode = false;
            this.completedSeriousDedication = false;
            this.completedCoverMeInDebris = false;

            this.allNetheriteAdvancementsComplete = false;

            this.craftedNetheriteBlock = false;
            this.placedNetheriteBlock = false;
        }

        protected override string GetShortStatus() => 
            this.EstimatedDebris.ToString();

        protected override string GetLongStatus()
        {
            if (this.allNetheriteAdvancementsComplete)
                return "Done\0With\nNetherite";

            if (this.placedNetheriteBlock)
                return "Netherite\nPlaced";

            if (this.EstimatedDebris >= Required || this.ManuallyChecked)
                return "All\0Debris\nCollected";
            
            return $"Debris:\0{this.EstimatedDebris}\nTNT:\0{Math.Max(this.EstimatedTnt, 0)}";
        }

        protected override string GetCurrentIcon()
        {
            if (Tracker.Category is AllBlocks)
                return "netherite_block";

            return this.CompletionOverride 
                ? "supporter_netherite" 
                : "obtain_ancient_debris";
        }
    }
}

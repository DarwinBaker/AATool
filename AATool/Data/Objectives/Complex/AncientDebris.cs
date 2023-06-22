using System;
using System.Xml;
using AATool.Configuration;
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

        public bool AllNetheriteAdvancementsComplete { get; private set; }
        public int EstimatedDebris { get; private set; }
        public int EstimatedTnt { get; private set; }

        protected bool CompletedHiddenInTheDepths;
        protected bool CompletedCountryLode;
        protected bool CompletedSeriousDedication;
        protected bool CompletedCoverMeInDebris;

        protected bool CraftedNetheriteBlock;
        protected bool PlacedNetheriteBlock;

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
                this.CraftedNetheriteBlock = progress.WasCrafted(NetheriteBlock);
                this.PlacedNetheriteBlock = progress.WasUsed(NetheriteBlock);
                this.CompletionOverride = this.PlacedNetheriteBlock;
            }
            else
            {
                //ignore count if all netherite related advancements are done
                this.CompletedHiddenInTheDepths = progress.AdvancementCompleted(ObtainDebris);
                this.CompletedCountryLode = progress.AdvancementCompleted(UseLodestone);
                this.CompletedSeriousDedication = progress.AdvancementCompleted(NetheriteHoe);
                this.CompletedCoverMeInDebris = progress.AdvancementCompleted(NetheriteArmor);

                this.AllNetheriteAdvancementsComplete = this.CompletedHiddenInTheDepths
                    && this.CompletedCountryLode
                    && this.CompletedSeriousDedication
                    && this.CompletedCoverMeInDebris;

                this.CompletionOverride = this.AllNetheriteAdvancementsComplete
                    || this.EstimatedDebris >= Required;
            }

            this.CanBeManuallyChecked = !this.CompletionOverride;
            if (this.ManuallyChecked)
                this.CompletionOverride = true;

            this.Partial = !this.AllNetheriteAdvancementsComplete;
        }

        protected override void ClearAdvancedState()
        {
            this.EstimatedDebris = 0;
            this.EstimatedTnt = 0;

            this.CompletedHiddenInTheDepths = false;
            this.CompletedCountryLode = false;
            this.CompletedSeriousDedication = false;
            this.CompletedCoverMeInDebris = false;

            this.AllNetheriteAdvancementsComplete = false;

            this.CraftedNetheriteBlock = false;
            this.PlacedNetheriteBlock = false;
        }

        protected override string GetShortStatus()
        {
            if (this.AllNetheriteAdvancementsComplete)
                return "Done";

            if (this.ManuallyChecked)
                return "Collected";

            return $"Debris: {this.EstimatedDebris}";
        }
            

        protected override string GetLongStatus()
        {
            if (this.AllNetheriteAdvancementsComplete)
                return "Done\0With\nNetherite";

            if (this.PlacedNetheriteBlock)
                return "Netherite\nPlaced";

            if (this.EstimatedDebris >= Required || this.ManuallyChecked)
                return "All\0Debris\nCollected";
            
            return $"Debris:\0{this.EstimatedDebris}\nTNT:\0{Math.Max(this.EstimatedTnt, 0)}";
        }

        protected override string GetCurrentIcon()
        {
            if (Tracker.Category is AllBlocks)
                return "netherite_block";

            //if (Config.Main.UseOptimizedLayout)
            //    return "obtain_ancient_debris";

            return this.AllNetheriteAdvancementsComplete
                ? "supporter_netherite" 
                : "obtain_ancient_debris";
        }
    }
}

using System;
using System.Collections.Generic;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class Sniffers : ComplexObjective
    {
        public const string ItemId = "minecraft:sniffer_egg";
        private const string Sniffer = "minecraft:sniffer";
        private const string ObtainEgg = "minecraft:husbandry/obtain_sniffer_egg";
        private const string TwoByTwo = "minecraft:husbandry/bred_all_animals";
        private const string LittleSniffs = "minecraft:husbandry/feed_snifflet";
        private const string PlantingThePast = "minecraft:husbandry/plant_any_sniffer_seed";

        public int EstimatedObtained { get; private set; }
        public int EstimatedPlaced { get; private set; }

        public int Required => (Version.TryParse(Tracker.CurrentVersion, out Version current) && current >= new Version("1.21")) 
            ? 2 : 3;

        private bool eggObtained;
        private bool sniffersBred;
        private bool sniffletFed;
        private bool seedPlanted;

        private bool doneWithSniffers;

        private readonly List<string> remainingObjectives = new List<string>();

        public Sniffers() : base() 
        {
            this.Name = "Sniffers";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.EstimatedObtained = progress.TimesPickedUp(ItemId)
                - progress.TimesDropped(ItemId)
                - progress.TimesUsed(ItemId);
            this.EstimatedObtained = Math.Max(0, this.EstimatedObtained);

            this.EstimatedPlaced = progress.TimesUsed(ItemId)
                - progress.TimesMined(ItemId);
            this.EstimatedPlaced = Math.Max(0, this.EstimatedPlaced);

            this.eggObtained = progress.AdvancementCompleted(ObtainEgg);
            this.sniffersBred = progress.CriterionCompleted(TwoByTwo, Sniffer);
            this.sniffletFed = progress.AdvancementCompleted(LittleSniffs);
            this.seedPlanted = progress.AdvancementCompleted(PlantingThePast);
            this.UpdateRemainingObjectives();

            this.doneWithSniffers = this.sniffersBred && this.sniffletFed && this.seedPlanted;

            this.Partial = !this.doneWithSniffers;
            this.CompletionOverride = this.EstimatedObtained >= this.Required
                || this.EstimatedPlaced > 0
                || this.doneWithSniffers;
        }

        protected override void ClearAdvancedState()
        {
            this.EstimatedObtained = 0;
            this.EstimatedPlaced = 0;
            this.UpdateRemainingObjectives();
        }

        private void UpdateRemainingObjectives()
        {
            this.remainingObjectives.Clear();
            if (!this.eggObtained)
                this.remainingObjectives.Add("Obtain\nEgg");
            if (!this.sniffersBred)
                this.remainingObjectives.Add("Must\0Breed\nSniffers");
            if (!this.sniffletFed)
                this.remainingObjectives.Add("Must\0Feed\nSnifflet");
            if (!this.seedPlanted)
                this.remainingObjectives.Add("Must\0Plant\nSeeds");
        }

        protected override string GetShortStatus()
        {
            if (this.doneWithSniffers)
                return "Done";

            if (this.remainingObjectives.Count is 1)
            {
                if (!this.eggObtained)
                    return "Get\0Egg";
                if (!this.sniffersBred)
                    return "Breed";
                if (!this.sniffletFed)
                    return "Feed";
                if (!this.seedPlanted)
                    return "Plant";
            }

            if (this.EstimatedPlaced > 0)
                return "Hatching";

            return $"{this.EstimatedObtained}\0/\0{this.Required}";
        }
        
        protected override string GetLongStatus()
        {
            if (this.doneWithSniffers)
                return "Done\0With\nSniffers";

            if (this.remainingObjectives.Count is 1)
                return this.remainingObjectives[0];

            if (this.EstimatedPlaced is 1)
                return $"Hatching\n{this.EstimatedPlaced}\0Egg";
            else if (this.EstimatedPlaced > 1)
                return $"Hatching\n{this.EstimatedPlaced}\0Eggs";

            return $"Eggs\n{this.EstimatedObtained}\0/\0{this.Required}";
        }

        protected override string GetCurrentIcon()
        {
            if (this.doneWithSniffers)
                return "sniffer_sniff";

            if (this.remainingObjectives.Count is 1)
            {
                if (!this.eggObtained)
                    return "obtain_sniffer_egg";
                if (!this.sniffletFed)
                    return "feed_snifflet";
                if (!this.seedPlanted)
                    return "plant_any_sniffer_seed";

                return "sniffer_mob";
            }

            return this.EstimatedPlaced > 0
                ? "sniffer_hatch"
                : "obtain_sniffer_egg";
        }
    }
}
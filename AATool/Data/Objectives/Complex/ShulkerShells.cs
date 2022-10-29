using System;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Complex
{
    class ShulkerShells : ComplexObjective
    {
        public const string ItemId = "minecraft:shulker_shell";

        public const int Required = 34;

        public static readonly string[] AllBoxVariants = new [] {
            "minecraft:white_shulker_box",
            "minecraft:red_shulker_box",
            "minecraft:orange_shulker_box",
            "minecraft:yellow_shulker_box",
            "minecraft:lime_shulker_box",
            "minecraft:green_shulker_box",
            "minecraft:cyan_shulker_box",
            "minecraft:light_blue_shulker_box",
            "minecraft:blue_shulker_box",
            "minecraft:purple_shulker_box",
            "minecraft:magenta_shulker_box",
            "minecraft:pink_shulker_box",
            "minecraft:brown_shulker_box",
            "minecraft:light_gray_shulker_box",
            "minecraft:gray_shulker_box",
            "minecraft:black_shulker_box",
            "minecraft:shulker_box",
        };

        public int EstimatedObtained { get; private set; }

        private bool allShulkerVariantsPlaced;

        public ShulkerShells() : base() 
        {
            this.Name = "ShulkerShells";
            this.Icon = "shulker_shell";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            if (Tracker.Category is AllBlocks)
            {
                this.EstimatedObtained = progress.TimesPickedUp(ItemId)
                    - progress.TimesDropped(ItemId)
                    - progress.TimesUsed(ItemId);
                this.EstimatedObtained = Math.Max(0, this.EstimatedObtained);
            }

            if (Tracker.Category is not AllBlocks)
            {
                this.CompletionOverride = false;
                return;
            }
            this.CompletionOverride = this.allShulkerVariantsPlaced = this.AllBoxesPlaced(progress);
        }

        private bool AllBoxesPlaced(ProgressState progress)
        {
            foreach (string sulkerBoxVariant in AllBoxVariants)
            {
                if (!progress.WasUsed(sulkerBoxVariant))
                    return false; 
            }
            return true;
        }

        protected override void ClearAdvancedState()
        {
            this.EstimatedObtained = 0;
            this.allShulkerVariantsPlaced = false;
        }

        protected override string GetShortStatus()
        { 
            return $"{this.EstimatedObtained}\0/\0{Required}";
        }

        protected override string GetLongStatus()
        {
            if (this.allShulkerVariantsPlaced)
                return "All Boxes Placed";
            else if (this.EstimatedObtained >= Required || this.ManuallyChecked)
                return "Finished Collecting";
            else
                return $"Shulkers\n{this.EstimatedObtained}\0/\0{Required}";
        }
    }
}

using System;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Complex
{
    class ShulkerShells : ComplexPickupObjective
    {
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

        public ShulkerShells() : base("minecraft:shulker_shell")
        { 
        }

        private bool allShulkerVariantsPlaced;

        public override int Required => 34;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            base.UpdateAdvancedState(progress);
            this.CompletionOverride |= this.allShulkerVariantsPlaced = this.EveryBlockPlaced(progress);
        }

        private bool EveryBlockPlaced(ProgressState progress)
        {
            foreach (string block in AllBoxVariants)
            {
                if (!progress.WasUsed(block))
                    return false; 
            }
            return true;
        }

        protected override void ClearAdvancedState()
        {
            base.ClearAdvancedState();
            this.allShulkerVariantsPlaced = false;
        }

        protected override string GetLongStatus()
        {
            if (this.allShulkerVariantsPlaced)
                return "All Boxes Placed";

            if (this.ManuallyChecked)
                return "Finished Collecting";
            
            return $"Shulkers\n{this.Obtained}\0/\0{this.Required}";
        }

        protected override string GetCurrentIcon()
        {
            return this.allShulkerVariantsPlaced
                ? "shulker_box"
                : "shulker_shell";
        }
    }
}

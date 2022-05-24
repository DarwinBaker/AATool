using System.Xml;
using AATool.Data.Categories;

namespace AATool.Data.Objectives.Pickups
{
    class ShulkerShell : Pickup
    {
        public const string ItemId = "minecraft:shulker_shell";

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

        private bool allShulkerVariantsPlaced;

        public ShulkerShell(XmlNode node) : base(node) { }

        protected override void HandleCompletionOverrides()
        {
            if (Tracker.Category is not AllBlocks)
            {
                this.CompletionOverride = false;
                return;
            }

            //check if all different colored shulkers have been placed
            this.allShulkerVariantsPlaced = true;
            foreach (string variant in AllBoxVariants)
            {
                if (!Tracker.TryGetBlock(variant, out Block box) || !box.IsComplete())
                {
                    this.allShulkerVariantsPlaced = false;
                    break;
                }
            }
            this.CompletionOverride = this.allShulkerVariantsPlaced;
        }

        protected override void UpdateLongStatus()
        {
            if (this.allShulkerVariantsPlaced)
                this.FullStatus = "All Boxes Placed";
            else if (this.PickedUp >= this.TargetCount)
                this.FullStatus = "Finished Collecting";
            else
                base.UpdateLongStatus();
        }
    }
}

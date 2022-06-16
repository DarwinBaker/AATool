using System;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Pickups
{
    class EGap : Pickup
    {
        public const string ItemId = "minecraft:enchanted_golden_apple";
        public const string BalancedDiet = "minecraft:husbandry/balanced_diet";
        public const string Overpowered = "achievement.overpowered";
        public const string EnchantedGoldenApple = "enchanted_golden_apple";
        public const string BannerRecipe = "minecraft:recipes/misc/mojang_banner_pattern";

        public bool Looted { get; private set; }
        public bool Eaten { get; private set; }

        public EGap(XmlNode node) : base(node) { }

        public override bool CompletedByAnyone() => base.CompletedByAnyone() || this.Looted;

        protected override void HandleCompletionOverrides()
        {
            //check if egap has been eaten
            if (Tracker.Category is AllAchievements)
            {
                Tracker.TryGetAdvancement(Overpowered, out Advancement overpowered);
                this.Eaten = overpowered?.IsComplete() is true;
            }
            else
            {
                Tracker.TryGetCriterion(BalancedDiet, EnchantedGoldenApple, out Criterion eatEgap);
                this.Eaten = eatEgap?.CompletedByDesignated() is true;
            }
            this.CompletionOverride = this.Eaten || this.ManuallyChecked;
        }

        public override void UpdateState(WorldState progress)
        {
            base.UpdateState(progress);
            if (Config.Tracking.Filter == ProgressFilter.Combined)
            {
                this.Looted = progress.AnyoneHasGodApple;
            }
            else
            {
                Player.TryGetUuid(Config.Tracking.SoloFilterName, out Uuid player);
                progress.Players.TryGetValue(player, out Contribution individual);
                this.Looted = individual?.HasGodApple is true;
            }

            if (Version.TryParse(Tracker.Category.CurrentMajorVersion, out Version current))
            {
                this.CanBeManuallyChecked = current <= new Version("1.12") && !(this.Looted || this.Eaten);
            }      
            this.UpdateLongStatus();
        }

        protected override void UpdateLongStatus()
        {
            if (this.Eaten)
                this.FullStatus = "God Apple Eaten";
            else if (this.PickedUp > 0 || this.Looted)
                this.FullStatus = "Obtained\nGod Apple";
            else
                this.FullStatus = "Obtain\nGod Apple";
        }

        protected override void UpdateShortStatus()
        {
            if (this.Eaten)
                this.ShortStatus = "Eaten";
            else if (this.PickedUp > 0 || this.Looted)
                this.ShortStatus = "Obtained";
            else
                base.UpdateShortStatus();
        }
    }
}

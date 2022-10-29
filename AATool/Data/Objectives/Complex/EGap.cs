using System;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Complex
{
    class EGap : ComplexObjective
    {
        public const string ItemId = "minecraft:enchanted_golden_apple";
        public const string BalancedDiet = "minecraft:husbandry/balanced_diet";
        public const string Overpowered = "achievement.overpowered";
        public const string EnchantedGoldenApple = "enchanted_golden_apple";
        public const string BannerRecipe = "minecraft:recipes/misc/mojang_banner_pattern";

        private static readonly Version TextureChanged = new ("1.14");
        private static readonly Version IdAdded = new ("1.12");

        private static bool ModernTexture => !Version.TryParse(Tracker.CurrentVersion, out Version current)
            || current >= TextureChanged;

        public bool Looted { get; private set; }
        public bool Eaten { get; private set; }

        public EGap() : base() 
        {
            this.Name = "EGap";
            this.RefreshIcon();
        }

        private void RefreshIcon()
        {
            this.Icon = ModernTexture ? "enchanted_golden_apple" : "enchanted_golden_apple_1.12";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.RefreshIcon();

            this.Looted = progress.ObtainedGodApple;

            this.Eaten = Tracker.Category is AllAchievements
                ? progress.AdvancementCompleted(Overpowered)
                : progress.CriterionCompleted(BalancedDiet, EnchantedGoldenApple);

            if (Version.TryParse(Tracker.Category.CurrentMajorVersion, out Version current))
                this.CanBeManuallyChecked = current <= IdAdded && !(this.Looted || this.Eaten);

            this.CompletionOverride = this.Looted || this.Eaten || this.ManuallyChecked;
        }

        protected override void ClearAdvancedState()
        {
            this.Looted = false;
            this.Eaten = false;

            if (Version.TryParse(Tracker.Category.CurrentMajorVersion, out Version current))
                this.CanBeManuallyChecked = current <= IdAdded && !(this.Looted || this.Eaten);
        }

        protected override string GetShortStatus()
        {
            if (this.Eaten)
                return "Eaten";
            else if (this.Looted)
                return "Obtained";
            else
                return "0";
        }

        protected override string GetLongStatus()
        {
            if (this.Eaten)
                return "God\0Apple\nEaten";
            else if (this.Looted || this.ManuallyChecked)
                return "Obtained\nGod\0Apple";
            else
                return "Obtain\nGod\0Apple";
        }

    }
}

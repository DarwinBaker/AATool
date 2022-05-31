using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Pickups
{
    class EGap : Pickup
    {
        public const string ItemId = "minecraft:enchanted_golden_apple";
        private const string BalancedDiet = "minecraft:husbandry/balanced_diet";
        private const string Overpowered = "achievement.overpowered";
        private const string EnchantedGoldenApple = "enchanted_golden_apple";

        public bool Eaten { get; private set; }

        public EGap(XmlNode node) : base(node) { }

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
            this.CanBeManuallyChecked = !(this.PickedUp > 0 || this.Eaten);
        }

        protected override void UpdateLongStatus()
        {
            if (this.Eaten)
                this.FullStatus = "God Apple Eaten";
            else if (this.PickedUp > 0 || this.ManuallyChecked)
                this.FullStatus = "Picked\0Up\nGod Apple";
            else
                this.FullStatus = "Pick\0Up\nGod Apple";
        }

        protected override void UpdateShortStatus()
        {
            if (this.Eaten)
                this.ShortStatus = "Eaten";
            else if (this.ManuallyChecked)
                this.ShortStatus = "Checked";
            else
                base.UpdateShortStatus();
        }
    }
}

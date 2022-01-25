using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AATool.Data.Objectives.Pickups
{
    class EGap : Pickup
    {
        public const string ItemId = "minecraft:enchanted_golden_apple";
        private const string BalancedDiet = "minecraft:husbandry/balanced_diet";
        private const string EnchantedGoldenApple = "enchanted_golden_apple";

        public EGap(XmlNode node) : base(node) { }

        protected override void HandleCompletionOverrides()
        {
            //check if egap has been eaten
            Tracker.TryGetCriterion(BalancedDiet, EnchantedGoldenApple, out Criterion eatEgap);
            this.CompletionOverride = eatEgap?.CompletedByDesignated() is true;
        }

        protected override void UpdateLongStatus()
        {
            if (this.CompletionOverride)
                this.FullStatus = "God Apple Eaten";
            else
                base.UpdateLongStatus();
        }

        protected override void UpdateShortStatus()
        {
            if (this.CompletionOverride)
                this.ShortStatus = "Eaten";
            else
                base.UpdateShortStatus();
        }
    }
}

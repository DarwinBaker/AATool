using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Complex
{
    class Foods : MultipartObjective
    {
        private const string Pufferfish = "Pufferfish";
        private const string SusStew = "Sus Stew";
        private const string GodApple = "God Apple";

        private bool onlyHdwghRemaining;

        public override string AdvancementId => "minecraft:husbandry/balanced_diet";
        public override string Criterion => "Food";
        public override string Action => "Eat";
        public override string PastAction => "Eaten";
        protected override string ModernBaseTexture => "balanced_diet";
        protected override string OldBaseTexture => "balanced_diet_1.12";

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            base.UpdateAdvancedState(progress);
            this.onlyHdwghRemaining = false;
            if (Tracker.Category is AllAdvancements
                && this.RemainingCriteria.Count > 0
                && this.RemainingCriteria.Count <= 3)
            {
                this.onlyHdwghRemaining = true;
                foreach (string food in this.RemainingCriteria)
                {
                    if (food is not (GodApple or Pufferfish or SusStew))
                        this.onlyHdwghRemaining = false;
                }
            }
        }

        protected override void ClearAdvancedState()
        {
            base.ClearAdvancedState();
            this.onlyHdwghRemaining = false;
        }

        protected override string GetLongStatus()
        {
            if (this.CompletionOverride)
                return $"All\0Food\nEaten";

            if (this.onlyHdwghRemaining)
                return "Awaiting\nHDWGH";

            if (this.OnLastCriterion)
                return $"Last\0Food:\n{this.RemainingCriteria.First()}";

            return $"Food\0Eaten\n{this.CurrentCriteria}\0/\0{this.RequiredCriteria}";
        }
    }
}

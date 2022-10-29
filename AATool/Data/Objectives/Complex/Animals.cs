
using System.Linq;

namespace AATool.Data.Objectives.Complex
{
    class Animals : ComplexCriteriaObjective
    {
        public Animals() : base()
        {
            this.Name = "Animals";
        }

        public override string AdvancementId => "minecraft:husbandry/bred_all_animals";
        public override string Criterion => "Animal";
        public override string Action => "Breed";
        public override string PastAction => "Bred";
        protected override string ModernTexture => "golden_carrot";
        protected override string OldTexture => "golden_carrot_1.12";

        protected override string GetLongStatus()
        {
            if (this.CompletionOverride)
                return $"All\0Animals\nBred";

            if (this.RemainingCriteria.Count is 1)
                return $"Last\0{this.Criterion}:\n{this.RemainingCriteria.First()}";

            return $"Animals\0Bred\n{this.CurrentCriteria}\0/\0{this.RequiredCriteria}";
        }
    }
}

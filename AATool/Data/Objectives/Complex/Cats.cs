using System.Linq;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class Cats : ComplexCriteriaObjective
    {
        private const string TwoByTwo = "minecraft:husbandry/bred_all_animals";
        private const string Cat = "minecraft:cat";

        public Cats() : base()
        {
            this.Name = "Cats";
            this.Icon = "complete_catalogue";
        }

        public override string AdvancementId => "minecraft:husbandry/complete_catalogue";
        public override string Criterion => "Cat";
        public override string Action => "Tame";
        public override string PastAction => "Tamed";
        protected override string ModernTexture => "complete_catalogue";
        protected override string OldTexture => "complete_catalogue";
        
        private bool catalogueComplete;
        private bool breedCats;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            base.UpdateAdvancedState(progress);
            this.catalogueComplete = progress.AdvancementCompleted(this.AdvancementId);
            this.breedCats = progress.CriterionCompleted(TwoByTwo, Cat);
            this.CompletionOverride &= this.breedCats;
        }


        protected override void ClearAdvancedState()
        {
            base.ClearAdvancedState();
            this.breedCats = false;
        }

        protected override string GetLongStatus()
        {
            if (this.CompletionOverride)
                return "Done\0With\nCats";

            if (this.RemainingCriteria.Count is 1)
                return $"Last\0Cat:\n{this.RemainingCriteria.First()}";
            
            if (this.catalogueComplete && !this.breedCats)
                return "Needs\0To\nBreed\0Cats";

            return $"Cats\0Tamed\n{this.CurrentCriteria}\0/\0{this.RequiredCriteria}";
        }
    }
}

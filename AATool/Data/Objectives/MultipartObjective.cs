using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Data.Progress;

namespace AATool.Data.Objectives
{
    public abstract class MultipartObjective : ComplexObjective
    {
        private static readonly Version VillageAndPillageUpdate = new ("1.14");

        public abstract string AdvancementId { get; }
        public abstract string Criterion { get; }
        public abstract string Action { get; }
        public abstract string PastAction { get; }

        protected abstract string ModernBaseTexture { get; }
        protected abstract string OldBaseTexture { get; }

        protected readonly HashSet<string> RemainingCriteria = new ();

        protected int RequiredCriteria;
        protected int CurrentCriteria;
        protected string LastCriterionIcon;

        protected virtual string LongStatusComplete() =>
            $"All\0{this.Criterion}s\n{this.PastAction}";
        protected virtual string LongStatusLast() =>
            $"Last\0{this.Criterion}:\n{this.RemainingCriteria.First()}";
        protected virtual string LongStatusNormal() =>
            $"{this.Action}\0{this.Criterion}s\n{this.CurrentCriteria}\0/\0{this.RequiredCriteria}";

        protected virtual Version TextureUpdateVersion => VillageAndPillageUpdate;

        protected bool UseModernTexture => !Version.TryParse(Tracker.CurrentVersion, out Version current)
            || current >= this.TextureUpdateVersion;

        protected string CurrentBaseTexture => this.UseModernTexture ? this.ModernBaseTexture : this.OldBaseTexture;

        protected bool OnLastCriterion => this.RemainingCriteria.Count is 1;

        protected bool AllCriteriaCompleted => this.RequiredCriteria > 0 
            && this.CurrentCriteria >= this.RequiredCriteria;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            if (Tracker.TryGetAdvancement(this.AdvancementId, out Advancement adv) && adv.HasCriteria)
            {
                this.BuildRemainingCriteriaList(adv.Criteria);
                this.CompletionOverride = adv.IsComplete();
            }
        }

        protected virtual void BuildRemainingCriteriaList(CriteriaSet criteria)
        {
            this.CurrentCriteria = 0;
            this.RequiredCriteria = criteria.Count;
            this.RemainingCriteria.Clear();
            foreach (Criterion criterion in criteria.All.Values)
            {
                if (criterion.IsComplete())
                { 
                    this.CurrentCriteria++;
                }
                else
                {
                    _= this.RemainingCriteria.Add(criterion.Name);
                    this.LastCriterionIcon = criterion.Icon;
                }
            }
        }

        protected override void ClearAdvancedState()
        {
            this.CurrentCriteria = 0;
            this.RemainingCriteria.Clear();
            if (Tracker.TryGetAdvancement(this.AdvancementId, out Advancement adv) && adv.HasCriteria)
                this.BuildRemainingCriteriaList(adv.Criteria);
        }

        protected override string GetShortStatus() => 
            $"{this.CurrentCriteria} / {this.RequiredCriteria}";

        protected void UpdateRequired()
        { 
            
        }

        protected override string GetLongStatus()
        {
            if (this.CompletionOverride)
                return this.LongStatusComplete();

            if (this.OnLastCriterion)
                return this.LongStatusLast();

            return this.LongStatusNormal();
        }

        protected override string GetCurrentIcon()
        {
            if (this.CompletionOverride || !this.OnLastCriterion)
                return this.CurrentBaseTexture;

            return this.LastCriterionIcon;
        }
    }
}

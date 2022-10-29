using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Data.Progress;

namespace AATool.Data.Objectives
{
    public abstract class ComplexCriteriaObjective : ComplexObjective
    {
        public abstract string AdvancementId { get; }
        public abstract string Criterion { get; }
        public abstract string Action { get; }
        public abstract string PastAction { get; }

        protected abstract string ModernTexture { get; }
        protected abstract string OldTexture { get; }

        protected static readonly Version TextureChanged = new ("1.14");

        protected readonly HashSet<string> RemainingCriteria = new ();

        protected int RequiredCriteria;
        protected int CurrentCriteria;
        protected string LastCriterionIcon;

        protected bool OnLastCriterion => this.RemainingCriteria.Count is 1;

        public ComplexCriteriaObjective() : base()
        {
            this.RefreshIcon();
        }

        protected virtual string LongStatusComplete() =>
            $"All\0{this.Criterion}s\n{this.PastAction}";
        protected virtual string LongStatusLast() =>
            $"Last\0{this.Criterion}:\n{this.RemainingCriteria.First()}";
        protected virtual string LongStatusNormal() =>
            $"{this.Action}\0{this.Criterion}s\n{this.CurrentCriteria}\0/\0{this.RequiredCriteria}";
        
        protected static bool UseModernTexture => !Version.TryParse(Tracker.CurrentVersion, out Version current)
            || current >= TextureChanged;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            if (Tracker.TryGetAdvancement(this.AdvancementId, out Advancement adv) && adv.HasCriteria)
            {
                this.BuildRemainingCriteriaList(adv.Criteria);
                this.CompletionOverride = adv.IsComplete();
            }
            this.RefreshIcon();
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
            {
                this.RequiredCriteria = adv.Criteria.Count;
                foreach (Criterion criterion in adv.Criteria.All.Values)
                    _= this.RemainingCriteria.Add(criterion.Name);
            }
            this.RefreshIcon();
        }

        protected override string GetShortStatus()
        {
            return $"{this.CurrentCriteria} / {this.RemainingCriteria}";
        }

        protected override string GetLongStatus()
        {
            if (this.CompletionOverride)
                return this.LongStatusComplete();

            if (this.OnLastCriterion)
                return this.LongStatusLast();

            return this.LongStatusNormal();
        }
        
        protected virtual void RefreshIcon()
        {
            if (this.CompletionOverride || !this.OnLastCriterion)
                this.Icon = UseModernTexture ? this.ModernTexture : this.OldTexture;
            else 
                this.Icon = this.LastCriterionIcon;
        }
    }
}

using System.Collections.Generic;
using AATool.Data.Objectives;

namespace AATool.Data.Categories
{
    public abstract class SingleAdvancement : Category
    {
        public Advancement Requirement { get; protected set; }

        public IEnumerable<Criterion> AllCriteria => this.Requirement.Criteria.All.Values;
        public override IEnumerable<Objective> GetOverlayObjectives() => this.AllCriteria;

        public override int GetTargetCount() => this.Requirement?.Criteria.Count ?? 0;
        public override int GetCompletedCount() => this.Requirement?.Criteria.MostCompleted ?? 0;

    }
}

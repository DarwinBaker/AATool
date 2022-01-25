using System.Collections.Generic;
using AATool.Data.Objectives;

namespace AATool.Data.Categories
{
    public abstract class SingleAdvancement : Category
    {
        protected Advancement Advancement;
        public IEnumerable<Criterion> AllCriteria => this.Advancement.Criteria.All.Values;
    }
}

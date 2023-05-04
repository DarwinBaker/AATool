using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AATool.Data.Progress;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    internal class ArmorTrimCriterion : Criterion
    {
        public string Recipe { get; private set; }
        public bool Obtained { get; private set; }
        public bool Applied => base.CompletedByDesignated();

        public override bool CompletedByDesignated() => this.Obtained || base.CompletedByDesignated();
        public override bool IsComplete() => this.Obtained || this.Applied;

        private string plainName;

        public ArmorTrimCriterion(XmlNode node, Advancement advancement) : base(node, advancement)
        {
            this.Recipe = XmlObject.Attribute(node, "recipe", string.Empty);
            this.plainName = this.Name;
        }

        public override void UpdateState(ProgressState progress)
        {
            base.UpdateState(progress);
            Obtained = progress.Recipes.ContainsKey(this.Recipe);
            //this.Name = Applied ? $"{this.plainName} (Applied)" : this.plainName;
            //progress.Recipes.TryGetValue();
        }
    }
}

using AATool.Data.Progress;
using AATool.Net;
using AATool.Utilities;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace AATool.Data.Objectives
{
    public class Criterion : Objective
    {
        public readonly Advancement Owner;
        
        public Uuid DesignatedPlayer => this.Owner.DesignatedPlayer;
        public string OwnerId => this.Owner.Id;

        public bool CompletedByDesignated() => this.CompletedBy(this.DesignatedPlayer);
        public override string GetFullCaption() => this.Name;
        public override string GetShortCaption() => this.ShortName;

        public Criterion(XmlNode node, Advancement advancement) : base (node)
        {
            this.Owner = advancement;
            this.Id = XmlObject.Attribute(node, "id", string.Empty);
            this.Name = XmlObject.Attribute(node, "name", string.Empty);

            //construct name from id if not explicitly provided
            string implicitName = this.Id.Split(':').LastOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(this.Name))
                this.Name = Main.TextInfo.ToTitleCase(implicitName.Replace('_', ' ') ?? string.Empty);
            this.ShortName = XmlObject.Attribute(node, "short_name", this.Name);

            //construct icon from id if not explicitly provided
            this.Icon = XmlObject.Attribute(node, "icon", string.Empty);
            if (string.IsNullOrEmpty(this.Icon))
                this.Icon = implicitName.ToLower().Replace(' ', '_');
        }

        public override void UpdateState(WorldState progress)
        {
            base.UpdateState(progress);
        }
    }
}

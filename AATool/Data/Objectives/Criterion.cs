using System.Linq;
using System.Xml;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class Criterion : Objective
    {
        public static string Key(string advancement, string criterion) => $"{advancement} {criterion}";

        public readonly Advancement Owner;
        
        public Uuid DesignatedPlayer => this.Owner.DesignatedPlayer;
        public string OwnerId => this.Owner.Id;

        public bool CompletedByDesignated() => this.CompletedBy(this.DesignatedPlayer);

        public override string FullStatus => this.Name;
        public override string TinyStatus => this.ShortName;

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
    }
}

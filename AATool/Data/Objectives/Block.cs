using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class Block : Objective
    {
        public float LightLevel     { get; private set; }

        public bool Glows => this.LightLevel > 0;

        public override string GetFullCaption() => this.Name;
        public override string GetShortCaption() => this.ShortName;

        public Block(XmlNode node) : base (node)
        {
            this.LightLevel = XmlObject.Attribute(node, "light_level", 0f);
        }

        public override void UpdateState(WorldState progress)
        {
            /*
            List<Uuid> placers = progress.PlacersOf(this);
            if (placers.Any())
            {
                if (this.FirstToPlace == Uuid.Empty)
                    this.FirstToPlace = placers.First();
            }
            else
            {
                this.FirstToPlace = Uuid.Empty;
            }
            */
        }
    }
}
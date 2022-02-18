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
        public bool ManuallyCompleted { get; set; }

        public bool DoubleHeight { get; private set; }
        public float LightLevel  { get; private set; }

        public bool Glows => this.LightLevel > 0;

        public override string GetFullCaption() => this.Name;
        public override string GetShortCaption() => this.ShortName;

        public override bool CompletedByAnyone() => 
            this.FirstCompletionist != Uuid.Empty || this.ManuallyCompleted;

        public Block(XmlNode node) : base (node)
        {
            this.DoubleHeight = XmlObject.Attribute(node, "double_height", false);
            this.LightLevel = XmlObject.Attribute(node, "light_level", 0f);
        }

        public void ToggleManualOverride()
        {
            this.ManuallyCompleted ^= true;
            Tracker.Blocks.UpdateCount();
        }

        public override void UpdateState(WorldState progress)
        {
            List<Uuid> placers = progress.CompletionistsOf(this);
            if (placers.Any())
            {
                if (this.FirstCompletionist == Uuid.Empty)
                    this.FirstCompletionist = placers.First();
            }
            else
            {
                this.FirstCompletionist = Uuid.Empty;
            }
        }
    }
}
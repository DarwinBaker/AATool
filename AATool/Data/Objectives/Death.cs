using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class Death : Objective
    {
        public bool ManuallyCompleted { get; set; }

        public bool DoubleHeight { get; private set; }
        public float LightLevel  { get; private set; }

        public bool Glows => this.LightLevel > 0;

        public IEnumerable<string> Messages { get; internal set; }

        public override string GetFullCaption() => this.Name;
        public override string GetShortCaption() => this.ShortName;

        public override bool CompletedByAnyone() => 
            this.FirstCompletionist != Uuid.Empty || this.ManuallyCompleted;

        public Death(XmlNode node) : base (node)
        {
            this.DoubleHeight = XmlObject.Attribute(node, "double_height", false);
            this.LightLevel = XmlObject.Attribute(node, "light_level", 0f);
            this.Messages = XmlObject.Attribute(node, "messages", "").Split(',');
        }

        public void Clear()
        {
            this.FirstCompletionist = Uuid.Empty;
            this.ManuallyCompleted = false;
        }

        public void ToggleManualOverride()
        {
            this.ManuallyCompleted ^= true;
            Tracker.Category.Deaths.UpdateTotal();
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
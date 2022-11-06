using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Data.Progress;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class Block : Objective
    {
        public bool Highlighted { get; set; }
        public bool DoubleHeight { get; private set; }
        public float LightLevel { get; private set; }
        public bool PickedUp { get; private set; }
        public bool Obtained { get; private set; }
        public string SearchTags { get; private set; }

        public bool Glows => this.LightLevel > 0;

        public override string FullStatus => this.Name;
        public override string TinyStatus => this.Name;

        public bool HasBeenPlaced => !this.FirstCompletion.IsEmpty;

        public override bool CompletedByAnyone => this.HasBeenPlaced || this.ManuallyChecked;

        public void ToggleHighlight() => this.Highlighted ^= true;

        public override bool IsComplete() => this.CompletedByAnyone;

        public Block(XmlNode node) : base (node)
        {
            this.Id = $"minecraft:{node.Name}";
            this.DoubleHeight = XmlObject.Attribute(node, "double_height", false);
            this.LightLevel = XmlObject.Attribute(node, "light_level", 0f);
            this.SearchTags = XmlObject.Attribute(node, "tags", string.Empty);
        }

        public override void UpdateState(ProgressState progress)
        {
            if (progress is null)
            {
                this.FirstCompletion = default;
                this.PickedUp = false;
                this.Obtained = false;
                return;
            }

            this.PickedUp = progress.WasPickedUp(this.Id);
            HashSet<Completion> placers = progress.CompletionsOf(this);
            if (placers.Any())
            {
                if (this.FirstCompletion.IsEmpty)
                { 
                    this.FirstCompletion = placers.First();
                    this.Highlighted = false;
                }
            }
            else
            {
                this.FirstCompletion = default;
            }
            this.Obtained = this.PickedUp || this.HasBeenPlaced;
        }
    }
}
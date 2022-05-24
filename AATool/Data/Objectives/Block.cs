using System;
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
        public bool Highlighted { get; set; }
        public bool DoubleHeight { get; private set; }
        public float LightLevel { get; private set; }
        public int PickupCount { get; private set; }
        public bool Obtained { get; private set; }

        public bool Glows => this.LightLevel > 0;

        public override string GetFullCaption() => this.Name;
        public override string GetShortCaption() => this.ShortName;

        public bool HasBeenPlaced => this.FirstCompletion.who != Uuid.Empty;

        public override bool CompletedByAnyone() => this.HasBeenPlaced || this.ManuallyChecked;

        public bool PickedUpByAnyone() => this.PickupCount > 0;

        public void ToggleHighlight() => this.Highlighted ^= true;

        public Block(XmlNode node) : base (node)
        {
            this.Id = $"minecraft:{node.Name}";
            this.DoubleHeight = XmlObject.Attribute(node, "double_height", false);
            this.LightLevel = XmlObject.Attribute(node, "light_level", 0f);
        }

        public override void UpdateState(WorldState progress)
        {
            this.PickupCount = progress.PickedUp(this.Id);
            Dictionary<Uuid, DateTime> placers = progress.CompletionsOf(this);
            if (placers.Any())
            {
                if (this.FirstCompletion.who == Uuid.Empty)
                { 
                    this.FirstCompletion = (placers.First().Key, default);
                    this.Highlighted = false;
                }
            }
            else
            {
                this.FirstCompletion = default;
            }
            this.Obtained = this.PickedUpByAnyone() || this.HasBeenPlaced;
        }
    }
}
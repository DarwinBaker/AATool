using AATool.Data.Progress;
using AATool.Net;
using AATool.Utilities;
using System.Linq;
using System.Xml;

namespace AATool.Data.Objectives
{
    public class Pickup : Objective
    {
        public int PickedUp { get; private set; }
        public int Dropped { get; private set; }
        public int TargetCount { get; protected set; }
        public bool IsEstimate { get; protected set; }

        protected string FullStatus;
        protected string ShortStatus;
        protected bool CompletionOverride;

        public override bool CompletedByAnyone() => this.PickedUp >= this.TargetCount || this.CompletionOverride;
        public override bool CompletedBy(Uuid player) => this.CompletedByAnyone();

        public override string GetFullCaption() => this.FullStatus;
        public override string GetShortCaption() => this.ShortStatus;

        public Pickup(XmlNode node) : base(node)
        {
            this.Id = XmlObject.Attribute(node, "id", string.Empty);
            this.Name = XmlObject.Attribute(node, "name", string.Empty);
            this.IsEstimate = XmlObject.Attribute(node, "estimate", false);
            this.TargetCount = XmlObject.Attribute(node, "target_count", 0);

            this.Icon = XmlObject.Attribute(node, "icon", string.Empty);
            if (string.IsNullOrEmpty(this.Icon))
            {
                //remove "minecraft:" prefix if necessary
                this.Icon = this.Id.Split(':').LastOrDefault();
            }

            this.UpdateLongStatus();
            this.UpdateShortStatus();
        }

        public override void UpdateState(WorldState progress)
        {
            this.PickedUp = progress.PickedUp(this.Id);
            this.HandleCompletionOverrides();
            this.UpdateLongStatus();
            this.UpdateShortStatus();
        }

        protected virtual void UpdateLongStatus()
        {
            if (this.TargetCount is 1)
                this.FullStatus = this.Name;
            else
                this.FullStatus = $"{this.Name}\n{this.PickedUp}\0/\0{this.TargetCount}";
        }

        protected virtual void UpdateShortStatus()
        {
            if (this.IsEstimate && this.CompletedByAnyone())
            {
                this.ShortStatus = "Complete";
            }
            else
            {
                this.ShortStatus = this.TargetCount is 1
                    ? this.PickedUp.ToString()
                    : $"{this.PickedUp} / {this.TargetCount}";
            }
        }

        protected virtual void HandleCompletionOverrides()
        {

        }
    }
}

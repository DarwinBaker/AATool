using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public interface IObjective
    {
        public bool CompletedByAnyone();
        public bool CompletedBy(Uuid player);

        public string GetId();
        public string GetName();
        public string GetShortName();
        public string GetIcon();

        public string GetFullCaption();
        public string GetShortCaption();

        public void UpdateState(WorldState state);
    }

    public abstract class Objective : IObjective
    {
        public string Id        { get; protected set; }
        public string Icon      { get; protected set; }
        public string Name      { get; protected set; }
        public string ShortName { get; protected set; }

        public Uuid FirstCompletionist { get; protected set; }
        public readonly List<Uuid> Completionists;
        public readonly FrameType Frame;

        public abstract string GetFullCaption();
        public abstract string GetShortCaption();

        public virtual bool CompletedByAnyone() => this.Completionists.Any();
        public virtual bool CompletedBy(Uuid id) => this.Completionists.Contains(id);

        public string GetId() => this.Id;
        public string GetIcon() => this.Icon;
        public string GetName() => this.Name;
        public string GetShortName() => this.ShortName;

        public Objective() { }

        public Objective(XmlNode node)
        {
            if (node is null)
                throw new ArgumentNullException("Objective source cannot be null.");

            this.Completionists = new List<Uuid>();

            //parse properties from xml
            this.Id = XmlObject.Attribute(node, "id", string.Empty);
            this.Name = XmlObject.Attribute(node, "name", string.Empty);
            this.ShortName = XmlObject.Attribute(node, "short_name", this.Name);

            //parse icon
            this.Icon = XmlObject.Attribute(node, "icon", string.Empty);
            if (string.IsNullOrEmpty(this.Icon))
                this.Icon = this.Id.Split('/').LastOrDefault() ?? string.Empty;

            if (this is Pickup)
            {
                this.Frame = FrameType.Statistic;
            }
            else
            {
                //parse frame
                string frame = XmlObject.Attribute(node, "type", FrameType.Normal.ToString());
                frame = XmlObject.Attribute(node, "frame", frame);
                if (Enum.TryParse(frame, true, out FrameType parsed))
                    this.Frame = parsed;
            }
        }

        protected void UpdateFirstCompletionist()
        {
            //attempt to reset first completionist
            if (Tracker.Invalidated)
                this.FirstCompletionist = Uuid.Empty;

            //lock in first player to give credit to
            if (this.FirstCompletionist == Uuid.Empty && this.Completionists.Any())
                this.FirstCompletionist = this.Completionists.First();
        }

        public abstract void UpdateState(WorldState progress);
    }
}

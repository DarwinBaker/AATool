using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public interface IObjective
    {
        public bool CompletedByAnyone();
        public bool CompletedBy(Uuid player);

        public Uuid FirstToComplete();

        public DateTime WhenFirstCompleted();
        public DateTime WhenCompletedBy(Uuid player);

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

        public Dictionary<Uuid, DateTime> Completions { get; protected set; }
        public (Uuid who, DateTime when) FirstCompletion { get; protected set; }

        public readonly FrameType Frame;

        public abstract string GetFullCaption();
        public abstract string GetShortCaption();

        public bool IsComplete()
        {
            return Config.Tracking.Filter == ProgressFilter.Combined || Peer.IsRunning
                ? this.CompletedByAnyone()
                : this.CompletedBySoloPlayer();
        }

        public virtual bool CompletedByAnyone() => this.Completions.Any();

        public virtual bool CompletedBy(Uuid player) => this.Completions.ContainsKey(player);

        public virtual bool CompletedBySoloPlayer() =>
            Player.TryGetUuid(Config.Tracking.SoloFilterName, out Uuid player) ? this.CompletedBy(player) : false;

        public Uuid FirstToComplete() => this.FirstCompletion.who;

        public DateTime WhenFirstCompleted() => this.FirstCompletion.when;

        public DateTime WhenCompletedBy(Uuid player) =>
           this.Completions.TryGetValue(player, out DateTime completed) ? completed : default;

        public string GetId() => this.Id;
        public string GetIcon() => this.Icon;
        public string GetName() => this.Name;
        public string GetShortName() => this.ShortName;

        public Objective() { }

        public Objective(XmlNode node)
        {
            if (node is null)
                throw new ArgumentNullException("Objective source cannot be null.");

            this.Completions = new ();

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

        public virtual void UpdateState(WorldState progress) 
        {
            this.Completions = progress.CompletionsOf(this);
            KeyValuePair<Uuid, DateTime> first = this.Completions.OrderBy(e => e.Value).FirstOrDefault();
            this.FirstCompletion = (first.Key, first.Value);
        }
    }
}

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

        public void UpdateState(ProgressState state);
    }

    public abstract class Objective : IObjective
    {
        public string Id        { get; protected set; }
        public string Icon      { get; protected set; }
        public string Name      { get; protected set; }
        public string ShortName { get; protected set; }

        public bool CanBeManuallyChecked { get; protected set; }
        public bool CompletionOverride { get; protected set; }
        public bool ManuallyChecked { get; set; }

        public HashSet<Completion> Completions { get; protected set; }
        public Completion FirstCompletion { get; protected set; }

        public readonly FrameType Frame;

        public abstract string GetFullCaption();
        public abstract string GetShortCaption();

        public string GetId() => this.Id;
        public string GetIcon() => this.Icon;
        public string GetName() => this.Name;
        public string GetShortName() => this.ShortName;

        public virtual void ToggleManualCheck()
        {
            this.ManuallyChecked ^= true;

            ProgressState progress;
            if (Config.Tracking.Filter == ProgressFilter.Combined || Peer.IsRunning)
            {
                progress = Tracker.State;
            }
            else
            {
                Player.TryGetUuid(Config.Tracking.SoloFilterName, out Uuid player);
                Tracker.State.Players.TryGetValue(player, out Contribution individual);
                progress = individual;
            }
            this.UpdateState(progress);
        }

        public Objective() { }

        public Objective(XmlNode node = null)
        {
            this.Completions = new ();

            //parse properties from xml
            this.Id = XmlObject.Attribute(node, "id", string.Empty);
            this.Name = XmlObject.Attribute(node, "name", string.Empty);
            this.ShortName = XmlObject.Attribute(node, "short_name", this.Name);
            this.CanBeManuallyChecked = XmlObject.Attribute(node, "manual", this.CanBeManuallyChecked);

            //parse icon
            this.Icon = XmlObject.Attribute(node, "icon", string.Empty);
            if (string.IsNullOrEmpty(this.Icon))
                this.Icon = this.Id.Split('/').LastOrDefault() ?? string.Empty;

            if (this is ComplexObjective)
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

        public virtual bool IsComplete() => this.Completions.Any() || this.CompletionOverride;
        
        //public virtual bool CompletedBySoloPlayer() =>
        //    Player.TryGetUuid(Config.Tracking.SoloFilterName, out Uuid player) ? this.CompletedBy(player) : false;

        public Uuid FirstToComplete() => this.FirstCompletion.Player;
        public DateTime WhenFirstCompleted() => this.FirstCompletion.Timestamp;

        public virtual bool CompletedByAnyone() => this.Completions.Any();

        public bool CompletedBy(Uuid player)
        {
            foreach (Completion completion in this.Completions)
            {
                if (player == completion.Player)
                    return true;
            }
            return false;
        }

        public DateTime WhenCompletedBy(Uuid player)
        {
            foreach (Completion completion in this.Completions)
            {
                if (player == completion.Player)
                    return completion.Timestamp;
            }
            return default;
        }

        public virtual void UpdateState(ProgressState progress) 
        {
            if (Tracker.WorldChanged || Tracker.SavesFolderChanged || !Tracker.IsWorking)
                this.ManuallyChecked = false;

            if (progress is not null)
            {
                this.Completions = progress.CompletionsOf(this);
                Completion first = this.Completions.OrderBy(e => e.Timestamp).FirstOrDefault();
                this.FirstCompletion = first;
            }
            else
            {
                this.Completions.Clear();
                this.FirstCompletion = default;
            }
        }
    }
}

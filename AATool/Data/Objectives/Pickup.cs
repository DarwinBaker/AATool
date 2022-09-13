using AATool.Configuration;
using AATool.Data.Objectives.Pickups;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Utilities;
using System;
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

        public virtual int GetTotal() => Math.Max(this.PickedUp - this.Dropped, 0);

        public override bool CompletedByAnyone()
        {
            return this.TargetCount is 1
                ? this.PickedUp > 0 || this.CompletionOverride
                : this.GetTotal() >= this.TargetCount || this.CompletionOverride;
        }
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
            if (Tracker.WorldChanged || Tracker.SavesFolderChanged || !Tracker.IsWorking)
                this.ManuallyChecked = false;

            if (Config.Tracking.Filter == ProgressFilter.Combined)
            {
                this.PickedUp = progress.PickedUp(this.Id);
                this.Dropped = progress.Dropped(this.Id);
            }
            else
            {
                Player.TryGetUuid(Config.Tracking.SoloFilterName, out Uuid player);
                progress.Players.TryGetValue(player, out Contribution individual);

                int pickedUp = 0;
                individual?.ItemCounts.TryGetValue(this.Id, out pickedUp);
                this.PickedUp = pickedUp;

                int dropped = 0;
                individual?.ItemsDropped.TryGetValue(this.Id, out dropped);
                this.Dropped = dropped;
            }
            this.HandleCompletionOverrides();
            this.UpdateLongStatus();
            this.UpdateShortStatus();
        }

        protected virtual void UpdateLongStatus()
        {
            if (this.TargetCount is 1)
                this.FullStatus = this.Name;
            else
                this.FullStatus = $"{this.Name}\n{this.GetTotal()}\0/\0{this.TargetCount}";
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
                    ? this.GetTotal().ToString()
                    : $"{this.GetTotal()} / {this.TargetCount}";
            }
        }

        public static Pickup FromNode(XmlNode node)
        {
            string id = XmlObject.Attribute(node, "id", string.Empty);
            return id switch {
                WitherSkull.ItemId or WitherSkull.LegacyItemId => new WitherSkull(node),
                GoldBlocks.ItemId or GoldBlocks.LegacyItemId => new GoldBlocks(node),
                NautilusShell.ItemId => new NautilusShell(node),
                AncientDebris.ItemId => new AncientDebris(node),
                Trident.ItemId => new Trident(node),
                EGap.ItemId => new EGap(node),
                ShulkerShell.ItemId => new ShulkerShell(node),
                Mycelium.BlockId => new Mycelium(node),
                DeepslateEmerald.BlockId => new DeepslateEmerald(node),
                _ => new Pickup(node)
            };
        }
    }
}

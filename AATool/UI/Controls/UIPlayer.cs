using AATool.Data.Progress;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AATool.UI.Controls
{
    public class UIPlayer : UIPanel
    {
        public User Player { get; set; }

        private UIPlayerFace face;
        private UIFlowCarousel flow;
        private Dictionary<string, UITextBlock> itemCounts;

        public UIPlayer()
        {
            this.BuildFromSourceDocument();
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.face = this.First<UIPlayerFace>();
            this.face.PlayerId = this.Player.Id;
            this.face.Scale    = 4;
            this.First<UITextBlock>("label_name").SetText(this.Player.Name);
            this.First<UITextBlock>("label_pronouns").SetText(this.Player.Pronouns);

            this.flow = this.First<UIFlowCarousel>("items");
            this.itemCounts = new Dictionary<string, UITextBlock>();
            foreach (string item in Tracker.AllItems.Keys)
            {
                if (this.First(item) is UITextBlock label)
                    this.itemCounts[item] = label;
            }
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            this.UpdateItemCarousel();
        }

        private void UpdateItemCarousel()
        {
            if (Tracker.Progress.Players.TryGetValue(this.Player.Id, out Contribution contribution))
            {
                foreach (KeyValuePair<string, UITextBlock> label in this.itemCounts)
                {
                    int count = contribution.ItemCount(label.Key);
                    label.Value.SetText($"    x{count}");
                    if (count > 0)
                    {
                        if (label.Value.IsCollapsed)
                        {
                            label.Value.Expand();
                            this.flow.ReflowChildren();
                        }
                    }
                    else if (!label.Value.IsCollapsed)
                    {
                        label.Value.Collapse();
                        this.flow.ReflowChildren();
                    }
                }
            }
            else
            {
                foreach (UITextBlock label in this.itemCounts.Values)
                    label.Collapse();
                this.flow.ReflowChildren();
            }
        }

        public override void DrawThis(Display display)
        {
            display.DrawRectangle(this.Bounds, this.BackColor, this.BorderColor, 2);
        }
    }
}

using System.Collections.Generic;
using AATool.Configuration;
using AATool.Data;
using AATool.Graphics;
using AATool.Net.Requests;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UILeaderboard : UIFlowPanel
    {
        private readonly Dictionary<string, UIPersonalBest> runs;

        private UIFlowPanel flow;
        private bool downloaded;

        public UILeaderboard()
        {
            this.runs = new ();
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.BuildFromTemplate();
            this.flow = this.First<UIFlowPanel>("pb_list");
            base.InitializeRecursive(screen);
        }

        public void Clear()
        {
            this.flow.Children.Clear();
            this.runs.Clear();
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.downloaded && LeaderboardRequest.Downloaded)
            {
                this.Clear();
                this.downloaded = true;
            }

            if (this.runs.Count > LeaderboardRequest.MaxShown)
                return;

            bool changed = false;
            foreach (PersonalBest run in LeaderboardRequest.Runs)
            {
                if (!this.runs.ContainsKey(run.Runner))
                {
                    var control = new UIPersonalBest(run);
                    control.InitializeRecursive(this.Root());
                    this.runs[run.Runner] = control;
                    this.flow.AddControl(control);
                    changed = true;
                }
                if (this.runs.Count > LeaderboardRequest.MaxShown)
                    break;
            }
            if (changed)
                this.flow.ResizeRecursive(this.flow.Bounds);
        }

        public override void DrawRecursive(Canvas canvas)
        {
            base.DrawRecursive(canvas);

            if (this.SkipDraw)
                return;

            for (int i = 0; i < this.flow.Children.Count - 2; i++)
            {
                Rectangle bounds = this.flow.Children[i].Bounds;
                var splitter = new Rectangle(bounds.Left + 8, bounds.Bottom - 8, bounds.Width - 16, 2);
                canvas.DrawRectangle(splitter, Config.Main.BorderColor);
            }
        }
    }
}

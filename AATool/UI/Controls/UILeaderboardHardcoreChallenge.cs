using System;
using AATool.Data.Speedrunning;
using AATool.Net.Requests;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    internal class UILeaderboardHardcoreChallenge : UILeaderboard
    {
        public override bool LiveBoardAvailable => Leaderboard.History is not null;

        protected override void RequestRefresh()
        {
            if (!this.LiveBoardAvailable)
                new SpreadsheetRequest("history_aa_1.16", Paths.Web.AASheet, Paths.Web.PrimaryAAHistory).EnqueueOnce();
        }

        protected override void InvokeLeaderboardRefresh()
        {
            Leaderboard.Refresh(this.Category, this.Version);
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);
            this.Players.FlowDirection = FlowDirection.TopToBottom;
        }

        protected override void PopulateNormal()
        {
            this.Clear();
            this.ContainsMainPlayer = false;
            for (int i = 0; i < Math.Min(this.Rows, this.Board.Runs.Count); i++)
            {
                var control = new UIPersonalBestHardcoreChallenge(this.Board) 
                { 
                    IsSmall = this.IsSmall, 
                    Rank = i + this.ScrollOffset + 1 
                };
                Run run = this.Board.Runs[i + this.ScrollOffset];
                this.Runs[$"{run.Runner}-{i}"] = control;
                new AvatarRequest(Leaderboard.GetRealName(run.Runner)).EnqueueOnce();
                control.SetRun(run, true);
                control.InitializeRecursive(this.Root());
                this.Players.AddControl(control);
            }
            this.Players.ResizeRecursive(this.HideButton ? this.Bounds : this.Players.Bounds);
            this.First<UIGrid>()?.RemoveControl(this.Spinner);
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
        }

        protected override void TryScroll(int rows)
        {
            int totalRuns = this.Board?.Runs.Count ?? 0;
            int maxOffset = Math.Max(totalRuns - this.Runs.Count, 0);
            this.ScrollOffset = MathHelper.Clamp(this.ScrollOffset + rows, 0, maxOffset);
        }
    }
}

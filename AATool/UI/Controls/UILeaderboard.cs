using System;
using System.Collections.Generic;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Players;
using AATool.Graphics;
using AATool.Net;
using AATool.Net.Requests;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UILeaderboard : UIFlowPanel
    {
        public const int Rows = 6;
        private readonly Dictionary<string, UIPersonalBest> runs;

        private UITextBlock title;
        private UIFlowPanel flow;
        private UIPicture spinner;
        private string sourcePage;
        private bool upToDate;

        public UILeaderboard()
        {
            this.runs = new ();
        }

        public bool LiveBoardAvailable => SpreadsheetRequest.DownloadedPages.Contains(this.sourcePage);

        public override void InitializeRecursive(UIScreen screen)
        {
            this.BuildFromTemplate();
            this.title = this.First<UITextBlock>("title");
            this.flow = this.First<UIFlowPanel>("pb_list");
            this.spinner = this.First<UIPicture>("spinner");
            base.InitializeRecursive(screen);

            if (Tracker.Category is not AllAchievements)
            {
                this.title.SetText($"(Un)-Official {Tracker.Category.CurrentMajorVersion}" +
                    $"\n{Tracker.Category.Acronym} Leaderboard");
            }
            else
            {
                if (Tracker.Category.CurrentMajorVersion is "1.11")
                    this.title.SetText($"(Un)-Official 1.8-1.11\nAACH Leaderboard");
                else
                    this.title.SetText($"(Un)-Official 1.0-1.6\nAACH Leaderboard");
            }

            //attempt to populate with cached or already downloaded data
            if (Leaderboard.TryGet(Leaderboard.Current, out Leaderboard board))
                this.Populate(board);

            //request ota leaderboard refresh unless already downloaded
            this.sourcePage = Tracker.Category.CurrentVersion.StartsWith("1.16")
                ? Paths.Web.PrimaryVersionBoard
                : Paths.Web.OtherVersionsBoard;

            if (!this.LiveBoardAvailable)
            {
                new SpreadsheetRequest(Paths.Web.LeaderboardSpreadsheet, this.sourcePage).EnqueueOnce();
                new SpreadsheetRequest(Paths.Web.NicknameSpreadsheet).EnqueueOnce();
            }
        }

        public void Clear()
        {
            this.flow.Children.Clear();
            this.runs.Clear();
        }

        private void Populate(Leaderboard board)
        {
            this.Clear();
            PersonalBest run = null;
            for (int i = 0; i < Rows; i++)
            {
                bool claimed = board.Runs.Count > i;
                var control = new UIPersonalBest();
                if (claimed)
                {
                    run = board.Runs[i];
                    this.runs[run.Runner] = control;
                }
                control.SetRun(run, claimed);
                control.InitializeRecursive(this.Root());
                this.flow.AddControl(control);
            }
            this.flow.ResizeRecursive(this.flow.Bounds);
            this.First<UIGrid>()?.RemoveControl(this.spinner);
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.upToDate && this.LiveBoardAvailable)
            {
                if (Leaderboard.TryGet(Leaderboard.Current, out Leaderboard live))
                    this.Populate(live);
                this.upToDate = true;
            }
        }

        public override void DrawRecursive(Canvas canvas)
        {
            base.DrawRecursive(canvas);

            if (this.SkipDraw || this.runs.Count is 0)
                return;

            for (int i = 0; i < Math.Min(this.flow.Children.Count, Rows - 1); i++)
            {
                Rectangle bounds = this.flow.Children[i].Bounds;
                var splitter = new Rectangle(bounds.Left + 8, bounds.Bottom - 8, bounds.Width - 16, 2);
                canvas.DrawRectangle(splitter, Config.Main.BorderColor);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Xml;
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
        private readonly Dictionary<string, UIPersonalBest> runs;

        public const int MinimumRows = 6;

        public string Version { get; private set; }
        public int Rows { get; private set; } = MinimumRows;

        private UITextBlock title;
        private UIFlowPanel flow;
        private UIPicture spinner;
        private string sourcePage;
        private bool upToDate;

        public UILeaderboard()
        {
            this.runs = new ();
            this.BuildFromTemplate();
        }

        public bool LiveBoardAvailable => SpreadsheetRequest.DownloadedPages.Contains(this.sourcePage);

        public override void InitializeRecursive(UIScreen screen)
        {
            this.title = this.First<UITextBlock>("title");
            this.flow = this.First<UIFlowPanel>("pb_list");
            this.spinner = this.First<UIPicture>("spinner");
            base.InitializeRecursive(screen);

            if (string.IsNullOrEmpty(this.Version))
                this.Version = Tracker.Category.CurrentMajorVersion;

            this.UpdateTitle();

            //attempt to populate with cached or already downloaded data
            if (Leaderboard.TryGet(Leaderboard.Specific(this.Version), out Leaderboard board))
                this.Populate(board);

            //request ota leaderboard refresh unless already downloaded
            this.sourcePage = this.Version is "1.16"
                ? Paths.Web.PrimaryVersionBoard
                : Paths.Web.OtherVersionsBoard;

            if (!this.LiveBoardAvailable)
            {
                new SpreadsheetRequest(Paths.Web.LeaderboardSpreadsheet, this.sourcePage).EnqueueOnce();
                new SpreadsheetRequest(Paths.Web.NicknameSpreadsheet).EnqueueOnce();
            }
        }

        private void UpdateTitle()
        {
            if (Config.Main.FullScreenLeaderboards)
            {
                string name = this.Version is not "1.11" ? "All Advancements" : "All Achievements";
                this.title.SetText($"{name}\n{this.Version}");
            }
            else if (Tracker.Category is not AllAchievements)
            {
                this.title.SetText($"(Un)-Official {this.Version}" +
                    $"\n{Tracker.Category.Acronym} Leaderboard");
            }
            else
            {
                if (this.Version is "1.11")
                    this.title.SetText($"(Un)-Official 1.8-1.11\nAACH Leaderboard");
                else
                    this.title.SetText($"(Un)-Official 1.0-1.6\nAACH Leaderboard");
            }
        }

        private void Clear()
        {
            this.flow.Children.Clear();
            this.runs.Clear();
        }

        private void Populate(Leaderboard board)
        {
            this.Clear();
            PersonalBest run = null;
            for (int i = 0; i < this.Rows; i++)
            {
                bool claimed = i < board.Runs.Count;
                var control = new UIPersonalBest(board);
                if (claimed)
                {
                    run = board.Runs[i];
                    this.runs[run.Runner] = control;
                    Player.FetchIdentity(run.Runner);
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
                if (Leaderboard.TryGet(Leaderboard.Specific(this.Version), out Leaderboard live))
                    this.Populate(live);
                this.upToDate = true;
            }
        }

        public override void DrawRecursive(Canvas canvas)
        {
            base.DrawRecursive(canvas);

            if (this.SkipDraw || this.runs.Count is 0)
                return;

            for (int i = 0; i < Math.Min(this.flow.Children.Count, this.Rows - 1); i++)
            {
                Rectangle bounds = this.flow.Children[i].Bounds;
                var splitter = new Rectangle(bounds.Left + 8, bounds.Bottom - 8, bounds.Width - 16, 2);
                canvas.DrawRectangle(splitter, Config.Main.BorderColor);
            }
        }

        public override void ReadNode(XmlNode node)
        { 
            base.ReadNode(node);
            this.Version = Attribute(node, "version", this.Version);
            this.Version ??= string.Empty;
            this.Rows = Attribute(node, "rows", this.Rows);
        }
    }
}

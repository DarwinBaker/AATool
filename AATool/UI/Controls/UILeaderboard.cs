using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Speedrunning;
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

        public string Category { get; private set; }
        public string Version { get; private set; }
        public int Rows { get; private set; } = MinimumRows;

        private UITextBlock title;
        private UIFlowPanel flow;
        private UIPicture spinner;
        private string sourceSheet;
        private string sourcePage;
        private bool upToDate;

        public UILeaderboard()
        {
            this.runs = new ();
            this.BuildFromTemplate();
        }

        public bool LiveBoardAvailable => SpreadsheetRequest.DownloadedPages.Contains((this.sourceSheet, this.sourcePage));

        public override void InitializeRecursive(UIScreen screen)
        {
            this.title = this.First<UITextBlock>("title");
            this.flow = this.First<UIFlowPanel>("pb_list");
            this.spinner = this.First<UIPicture>("spinner");
            base.InitializeRecursive(screen);

            this.UpdateTitle();

            //attempt to populate with cached or already downloaded data
            if (Leaderboard.TryGet(this.Category, this.Version, out Leaderboard board))
                this.Populate(board);

            //request ota leaderboard refresh unless already downloaded
            if (this.Category is "All Blocks")
            {
                this.sourceSheet = Paths.Web.ABSheet;
                this.sourcePage = this.Version is "1.16" 
                    ? Paths.Web.ABPage16 
                    : Paths.Web.ABPage18;
            }
            else
            {
                this.sourceSheet = Paths.Web.AASheet;
                this.sourcePage = this.Version is "1.16"
                    ? Paths.Web.AAPage16
                    : Paths.Web.AAPageOthers;
            }

            if (!this.LiveBoardAvailable)
            {
                new SpreadsheetRequest(this.sourceSheet, this.sourcePage).EnqueueOnce();
                new SpreadsheetRequest(Paths.Web.NicknameSheet).EnqueueOnce();
            }
        }

        public override void UpdateRecursive(Time time)
        {
            this.UpdateThis(time);
            if (!this.IsCollapsed)
            {
                foreach (UIControl control in this.Children)
                    control.UpdateRecursive(time);
            }    
        }

        private void UpdateTitle()
        {
            string version = this.Version switch {
                "1.6" => "1.0-1.6",
                "1.11" => "1.8-1.11",
                _ => this.Version,
            };

            string title = Config.Main.ActiveTab != "tracker"
                ? $"{this.Category}\n{version}"
                : $"(Un)-Official {version}\n{Tracker.Category.Acronym} Leaderboard";

            this.title.SetText(title);
        }

        private void Clear()
        {
            this.flow.Children.Clear();
            this.runs.Clear();
        }

        private void Populate(Leaderboard board)
        {
            if (this.Category is "All Blocks" && board.Runs.Any())
            {
                this.Root().First<UIAvatar>($"ab_wr_{this.Version}_avatar")?.SetPlayer(board.Runs[0].Runner);
                this.Root().First<UIAvatar>($"ab_wr_{this.Version}_avatar")?.RegisterOnLeaderboard(board);
                this.Root().First<UIAvatar>($"ab_wr_{this.Version}_avatar")?.RefreshBadge();
                this.Root().First<UITextBlock>($"ab_wr_{this.Version}_runner")?.SetText(board.Runs[0].Runner);
                this.Root().First<UITextBlock>($"ab_wr_{this.Version}_igt")?.SetText(board.Runs[0].InGameTime.ToString());
                this.Root().First<UITextBlock>($"ab_wr_{this.Version}_blocks")?.SetText($"({board.Runs[0].Blocks} Blocks)");
                //int blocksPerHour = (int)(AllBlocks.GetTotalBlocksFor(this.Version) / board.Runs[0].InGameTime.TotalHours);
                //this.Root().First<UITextBlock>($"ab_wr_{this.Version}_blockrate")?.SetText($"{blocksPerHour} blocks/hr");
                return;
            }
            else if (Config.Main.ActiveTab == Config.RunnersTab)
            {
                UIFlowPanel list = this.Root().First<UIFlowPanel>("runner_list");
                if (list is not null)
                {
                    list.ClearControls();
                    for (int i = 0; i < board.Runs.Count; i++)
                    {
                        var control = new UIPersonalBest(board) { FlexWidth = new (140), IsSmall = true };
                        Run submission = board.Runs[i];
                        this.runs[submission.Runner] = control;
                        Player.FetchIdentityAsync(submission.Runner);
                        control.SetRun(submission, true);
                        control.InitializeRecursive(this.Root());
                        list.AddControl(control);
                    }
                }
                return;
            }

            this.Clear();
            Run run = null;
            for (int i = 0; i < this.Rows; i++)
            {
                bool claimed = i < board.Runs.Count;
                var control = new UIPersonalBest(board);
                if (claimed)
                {
                    run = board.Runs[i];
                    this.runs[run.Runner] = control;
                    Player.FetchIdentityAsync(run.Runner);
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
                if (Leaderboard.TryGet(this.Category, this.Version, out Leaderboard live))
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
            this.Category = Attribute(node, "category", Tracker.Category.Name);
            this.Version = Attribute(node, "version", Tracker.Category.CurrentMajorVersion);
            this.Rows = Attribute(node, "rows", this.Rows);
        }
    }
}

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
using AATool.UI.Badges;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UILeaderboard : UIFlowPanel
    {
        protected readonly Dictionary<string, UIPersonalBest> Runs;

        public const int MinimumRows = 6;

        public string Category { get; private set; }
        public string Version { get; private set; }
        public int Rows { get; private set; } = MinimumRows;

        private readonly Timer scrollWaitTimer = new (0.25, true);
        private readonly Timer scrollHoldTimer = new (0.05, true);

        protected Leaderboard Board;

        protected UIFlowPanel Players;
        protected UITextBlock Title;
        protected UIFlowPanel Menu;

        protected UIPicture Spinner;
        protected UIPicture ThreeDots;

        protected UIButton UpButton;
        protected UIPicture UpIcon;

        protected UIButton DownButton;
        protected UIPicture DownIcon;

        protected UIButton TopButton;
        protected UIPicture TopIcon;

        private UIButton recenterButton;
        private UIPicture recenterOutline;
        private UIPicture recenterIcon;

        private UIButton refreshButton;
        private UIPicture refreshIcon;

        protected string SourceSheet;
        protected string SourcePage;
        protected string MostRecords;
        protected bool NeedsLayoutRefresh;
        protected bool UpToDate;
        protected bool HideButton;
        protected bool IsSmall;
        protected bool ContainsMainPlayer;
        protected bool Snapshot;
        protected int MainPlayerOffset;
        protected int ScrollOffset;

        public UILeaderboard()
        {
            this.Runs = new();
            this.BuildFromTemplate();
        }

        public virtual bool LiveBoardAvailable => SpreadsheetRequest.DownloadedPages.Contains((this.SourceSheet, this.SourcePage));

        public void ScrollUp(int rows) => this.TryScroll(-rows);
        public void ScrollDown(int rows) => this.TryScroll(rows);

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);

            if (this.HideButton)
                this.First<UIGrid>()?.CollapseRow(0);

            this.UpButton = this.First<UIButton>("up");
            this.UpButton.OnClick += this.OnClick;
            this.UpIcon = this.UpButton?.First<UIPicture>();

            this.DownButton = this.First<UIButton>("down");
            this.DownButton.OnClick += this.OnClick;
            this.DownIcon = this.DownButton?.First<UIPicture>();

            this.TopButton = this.First<UIButton>("top");
            this.TopButton.OnClick += this.OnClick;
            this.TopIcon = this.TopButton?.First<UIPicture>();

            this.recenterButton = this.First<UIButton>("recenter");
            this.recenterButton.OnClick += this.OnClick;
            this.recenterOutline = this.recenterButton?.First<UIPicture>("recenter_outline");
            this.recenterIcon = this.recenterButton?.First<UIPicture>("recenter_avatar");

            this.refreshButton = this.First<UIButton>("refresh");
            this.refreshButton.OnClick += this.OnClick;
            this.refreshIcon = this.refreshButton?.First<UIPicture>();

            //this.recenterButton = this.First<UIButton>("recenter");
            //this.recenterButton.OnClick += this.OnClick;

            this.Title = this.First<UITextBlock>("title");
            this.Players = this.First<UIFlowPanel>("pb_list");
            this.Menu = this.First<UIFlowPanel>("menu");
            this.Spinner = this.First<UIPicture>("spinner");
            this.ThreeDots = this.First<UIPicture>("three_dots");

            //this.button.SetVisibility(UIMainScreen.ActiveTab == UIMainScreen.TrackerTab);
            //this.title.SetVisibility(UIMainScreen.ActiveTab != UIMainScreen.TrackerTab);

            if (this.IsSmall)
                this.First<UIGrid>().SetRowHeight(0, new (36));

            this.UpdateTitle();
            
            //attempt to populate with cached data
            if (Leaderboard.TryGet(this.Category, this.Version, out Leaderboard board))
            {
                this.Board = board;
                this.ScrollOffset = this.MainPlayerOffset;
                this.Populate();
                bool performScroll = UIMainScreen.ActiveTab == UIMainScreen.TrackerTab;
                this.UpdateMainPlayerScroll(Tracker.GetMainPlayer(), performScroll);
            }
            else
            {
                this.UpdateMenu();
            }

            string version = this.Version?.ToLower() ?? string.Empty;
            this.Snapshot = version.Contains("snapshot") || version.Contains("w");
            if (this.Snapshot)
            { 
                this.First<UIGrid>()?.RemoveControl(this.Spinner);
                var message = new UITextBlock() {
                    Row = 1,
                };
                message.SetFont("minecraft", 24);
                message.SetText("No Leaderboard For Snapshots");
                this.First<UIGrid>().AddControl(message);
                this.UpdateMenu();
            }

            //request ota leaderboard refresh unless already downloaded
            this.RequestRefresh();
        }

        protected virtual void RequestRefresh()
        {
            if (this.Category is "All Blocks")
            {
                this.SourceSheet = Paths.Web.ABSheet;
                this.SourcePage = this.Version switch {
                    "1.16" => Paths.Web.ABPage16,
                    "1.18" => Paths.Web.ABPage18,
                    "1.19" => Paths.Web.ABPage19,
                    "1.20" => Paths.Web.ABPage20,
                    _ => string.Empty
                };
            }
            else if (this.Category is "All Advancements")
            {
                this.SourceSheet = Paths.Web.AASheet;
                this.SourcePage = this.Version is "1.16" ? Paths.Web.AAPage16 : Paths.Web.AAPageOthers;
            }
            else 
            {
                this.SourceSheet = Paths.Web.ABSheet;
                this.SourcePage = Paths.Web.ABPageChallenges;
            }

            if (!this.LiveBoardAvailable)
            {
                new SpreadsheetRequest($"{this.Version} {this.Category}", this.SourceSheet, this.SourcePage).EnqueueOnce();
                new SpreadsheetRequest("Player Nicknames", Paths.Web.NicknameSheet).EnqueueOnce();
            }
        }

        protected virtual void InvokeLeaderboardRefresh()
        { 
            Leaderboard.Refresh(this.Category, this.Version);
        }

        private void OnClick(UIControl sender)
        {
            int oldOffset = this.ScrollOffset;
            if (sender == this.refreshButton)
            {
                this.InvokeLeaderboardRefresh();
                this.RequestRefresh();
                UIMainScreen.ForceLayoutRefresh();
            }
            else if (sender == this.UpButton)
            {
                this.ScrollUp(1);
            }
            else if (sender == this.DownButton)
            {
                this.ScrollDown(1);
            }
            else if (sender == this.TopButton)
            {
                this.ScrollOffset = 0;
            }
            else if (sender == this.recenterButton)
            {
                this.ScrollOffset = this.MainPlayerOffset;
            }
            this.NeedsLayoutRefresh = this.ScrollOffset != oldOffset;
        }

        private void UpdateMainPlayerScroll(Uuid mainPlayer, bool performScroll)
        {
            string name;
            if (Player.NameCache.Any())
                Player.TryGetName(mainPlayer, out name);
            else
                name = Config.Tracking.LastPlayer;

            if (Leaderboard.TryGetRank(name, this.Category, this.Version, out int rank) && rank > 0)
            {
                this.recenterIcon.SetTexture($"avatar-{name.ToLower()}");
                int totalRuns = this.Board?.Runs.Count ?? 0;
                int maxOffset = Math.Max(totalRuns - this.Runs.Count, 0);
                this.MainPlayerOffset = MathHelper.Clamp(rank - 3, 0, maxOffset);
                if (performScroll)
                {
                    this.ScrollOffset = this.MainPlayerOffset;
                    this.NeedsLayoutRefresh = true;
                }
            }
            else
            {
                this.recenterIcon.SetTexture(string.Empty);
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
            string acronym = this.Category switch {
                "All Advancements" => "AA",
                "All Achievements" => "AACH",
                "All Blocks" => "AB",
                _ => string.Empty
            };

            if (this.IsSmall)
            {
                string version = this.Version switch {
                    "1.6" => "1.0-1.6",
                    "1.11" => "1.8-1.11",
                    _ => this.Version,
                };
                this.Title.SetText($"{version}");
                return;
            }

            string header = this.Version switch {
                "1.6" => "Unofficial 1.0-1.6\nAACH Leaderboard",
                "1.11" => "Unofficial 1.8-1.11\nAACH Leaderboard",
                _ => $"The Unofficial {this.Version}\n{acronym} Leaderboard",
            };
            this.Title.SetText(header);
        }

        protected void Clear()
        {
            this.Players.Children.Clear();
            this.Runs.Clear();
        }

        private void Populate()
        {
            if (this.Category is "All Blocks" && this.DrawMode is DrawMode.None)
            {
                this.PopulateAllBlocks();
            }
            //else if (UIMainScreen.ActiveTab is UIMainScreen.RunnersTab)
            //{
            //    this.PopulateSmall();
            //}
            else
            { 
                if (!this.Snapshot)
                    this.PopulateNormal();
                if (UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
                    this.PopulateMultiboard();
            }
            this.UpdateMenu();
            this.NeedsLayoutRefresh = false;
        }

        private void PopulateAllBlocks()
        {
            if (this.Board.Runs.Any())
            {
                Run wr = this.Board.Runs[0];

                if (this.Root().TryGetFirst(out UIButton button, $"ab_wr_{this.Version}_button"))
                    button.Tag = wr.Runner;

                new AvatarRequest(wr.Runner).EnqueueOnce();

                this.Root().First<UIAvatar>($"ab_wr_{this.Version}_avatar")?.SetPlayer(wr.Runner);
                this.Root().First<UIAvatar>($"ab_wr_{this.Version}_avatar")?.RegisterOnLeaderboard(this.Board);
                this.Root().First<UIAvatar>($"ab_wr_{this.Version}_avatar")?.RefreshBadge();
                this.Root().First<UITextBlock>($"ab_wr_{this.Version}_runner")?.SetText(wr.Runner);
                this.Root().First<UITextBlock>($"ab_wr_{this.Version}_igt")?.SetText((int)wr.InGameTime.TotalHours + wr.InGameTime.ToString("':'mm':'ss"));
                this.Root().First<UITextBlock>($"ab_wr_{this.Version}_blocks")?.SetText($"({wr.ExtraStat} Blocks)");
            }
        }

        private void PopulateMultiboard()
        {
            string mostRecordsList = string.Empty;
            for (int i = 0; i < Leaderboard.ListOfMostConcurrentRecords.Count; i++)
            {
                mostRecordsList += Leaderboard.ListOfMostConcurrentRecords[i].GameVersion;
                if (i < Leaderboard.ListOfMostConcurrentRecords.Count - 1)
                    mostRecordsList += ", ";
            }
            //UIAvatar allBlocks20 = this.Root().First<UIAvatar>("ab_wr_1.20_avatar");
            //allBlocks20.SetBadge(new RankBadge(1, "All Blocks", "1.20", false));

            UIAvatar mostRecords = this.Root().First<UIAvatar>("most_records_avatar");
            mostRecords?.SetPlayer(Leaderboard.RunnerWithMostConcurrentRecords);
            this.Root().First<UITextBlock>("most_records_runner")?.SetText(Leaderboard.RunnerWithMostConcurrentRecords);
            this.Root().First<UITextBlock>("most_records_list")?.SetText(mostRecordsList);

            if (!string.IsNullOrEmpty(Leaderboard.AnyRsgRunner))
            {
                this.Root().First<UIAvatar>("any%_rsg_avatar")?.SetPlayer(Leaderboard.AnyRsgRunner);
                this.Root().First<UITextBlock>("any%_rsg_runner")?.SetText(Leaderboard.AnyRsgRunner);
                this.Root().First<UITextBlock>("any%_rsg_time")?.SetText($"{Leaderboard.AnyRsgInGameTime:m':'ss} IGT    {Leaderboard.AnyRsgRealTime:m':'ss} RTA");
            }
            if (!string.IsNullOrEmpty(Leaderboard.AnySsgRunner))
            {
                this.Root().First<UIAvatar>("any%_ssg_avatar")?.SetPlayer(Leaderboard.AnySsgRunner);
                this.Root().First<UITextBlock>("any%_ssg_runner")?.SetText(Leaderboard.AnySsgRunner);
                this.Root().First<UITextBlock>("any%_ssg_time")?.SetText($"{Leaderboard.AnySsgInGameTime:m':'ss} IGT    {Leaderboard.AnySsgRealTime:m':'ss} RTA");
            }
            if (!string.IsNullOrEmpty(Leaderboard.AASsgRunner))
            {
                this.Root().First<UIAvatar>("aa_ssg_avatar")?.SetPlayer(Leaderboard.AASsgRunner);
                this.Root().First<UITextBlock>("aa_ssg_runner")?.SetText(Leaderboard.AASsgRunner);
                this.Root().First<UITextBlock>("aa_ssg_time")?.SetText($"{Leaderboard.AASsgInGameTime:h':'mm':'ss} IGT   {Leaderboard.AASsgRealTime:h':'mm':'ss} RTA");
            }
        }

        private void PopulateSmall()
        {
            this.Clear();
            UIFlowPanel list = this.Root().First<UIFlowPanel>("runner_list");
            if (list is not null && list.Children.Count is 0)
            {
                list.ClearControls();
                for (int i = 0; i < this.Board.Runs.Count; i++)
                {
                    var control = new UIPersonalBest(this.Board) { FlexWidth = new (150), IsSmall = true };
                    Run run = this.Board.Runs[i + this.ScrollOffset];
                    this.Runs[run.Runner] = control;
                    new AvatarRequest(Leaderboard.GetRealName(run.Runner)).EnqueueOnce();

                    control.SetRun(run);
                    control.InitializeRecursive(this.Root());
                    list.AddControl(control);
                }
            }
            list.ReflowChildren();
        }

        protected virtual void PopulateNormal()
        {
            this.Clear();
            this.ContainsMainPlayer = false;
            Run run = null;
            for (int i = 0; i < this.Rows; i++)
            {
                bool claimed = i < this.Board.Runs.Count;
                var control = new UIPersonalBest(this.Board) { IsSmall = this.IsSmall, Rank = i + this.ScrollOffset + 1 };
                if (claimed)
                {
                    run = this.Board.Runs[i + this.ScrollOffset];
                    this.Runs[run.Runner] = control;
                    new AvatarRequest(Leaderboard.GetRealName(run.Runner)).EnqueueOnce();
                }
                else
                {
                    run = null;
                    control.SetRank(i + 1);
                }
                control.SetRun(run, claimed);
                control.InitializeRecursive(this.Root());
                this.Players.AddControl(control);
                this.ContainsMainPlayer |= control.IsMainPlayer;
            }
            this.Players.ResizeRecursive(this.HideButton ? this.Bounds : this.Players.Bounds);
            this.First<UIGrid>()?.RemoveControl(this.Spinner);
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.Players.ResizeRecursive(this.HideButton ? this.Bounds : this.Players.Bounds);
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.UpToDate && this.LiveBoardAvailable)
            {
                if (Leaderboard.TryGet(this.Category, this.Version, out Leaderboard live))
                {
                    this.Board = live;
                    this.Populate();
                    this.UpdateMenu();
                }  
                this.UpToDate = true;
            }

            if (Config.Tracking.FilterChanged)
                UIMainScreen.Invalidate();

            if (Tracker.MainPlayerChanged)
            {
                bool performScroll = UIMainScreen.ActiveTab == UIMainScreen.TrackerTab;
                this.UpdateMainPlayerScroll(Tracker.GetMainPlayer(), performScroll);
            }    

            if (this.NeedsLayoutRefresh)
                this.Populate();

            if (Config.Main.AppearanceChanged)
                this.UpdateMenu();

            this.UpdateScrollWheel();
            this.UpdateHoldScroll(time);
        }

        private void UpdateMenu()
        {
            bool available = Leaderboard.IsLiveAvailable(this.Category, this.Version);
            this.ThreeDots.SetVisibility(!this.Snapshot && !available);

            bool menuChanged = false;
            menuChanged |= this.TopButton.IsCollapsed == (this.ScrollOffset > 0);
            menuChanged |= this.refreshButton.IsCollapsed == available;
            menuChanged |= this.recenterButton.IsCollapsed == (this.ScrollOffset != this.MainPlayerOffset);

            int runs = this.Board?.Runs.Count ?? 0;
            this.UpButton.SetVisibility(this.Rows < runs);
            this.DownButton.SetVisibility(this.Rows < runs);
            this.TopButton.SetVisibility(this.Rows < runs);
            this.recenterButton.SetVisibility(this.Rows < runs);

            this.UpButton.Enabled = this.ScrollOffset > 0;
            this.TopButton.Enabled = this.ScrollOffset > 0;

            int totalRuns = this.Board?.Runs.Count ?? 0;
            int maxOffset = Math.Max(totalRuns - this.Runs.Count, 0);
            this.DownButton.Enabled = this.ScrollOffset < maxOffset;

            this.refreshButton.Enabled = available;
            this.recenterButton.SetVisibility(!this.ContainsMainPlayer && !string.IsNullOrEmpty(this.recenterIcon.Texture));

            Color dim = ColorHelper.Fade(Config.Main.TextColor, 0.1f);
            this.UpIcon.SetTint(this.UpButton.Enabled ? Config.Main.TextColor : dim);
            this.DownIcon.SetTint(this.DownButton.Enabled ? Config.Main.TextColor : dim);
            this.TopIcon.SetTint(this.TopButton.Enabled ? Config.Main.TextColor : dim);
            this.refreshIcon.SetTint(this.refreshButton.Enabled ? Config.Main.TextColor : dim);
            this.recenterOutline.SetTint(Config.Main.TextColor);

            if (menuChanged)
                this.Menu.ReflowChildren();
        }

        private void UpdateHoldScroll(Time time)
        {
            if (Input.LeftClicking)
            {
                this.scrollWaitTimer.Update(time);
                this.scrollHoldTimer.Update(time);
                Point cursor = Input.Cursor(this.Root());
                if (this.scrollWaitTimer.IsExpired && this.scrollHoldTimer.IsExpired)
                {
                    int oldOffset = this.ScrollOffset;
                    if (this.UpButton.Bounds.Contains(cursor))
                    {
                        this.ScrollUp(1);
                        this.scrollHoldTimer.Reset();
                    }
                    else if (this.DownButton.Bounds.Contains(cursor))
                    {
                        this.ScrollDown(1);
                        this.scrollHoldTimer.Reset();
                    }
                    this.NeedsLayoutRefresh = this.ScrollOffset != oldOffset;
                }
                
            }
            else if (Input.LeftClicked)
            {
                this.scrollWaitTimer.Reset();
                this.scrollHoldTimer.Reset();
            }
        }

        public override void DrawRecursive(Canvas canvas)
        {
            base.DrawRecursive(canvas);

            if (this.SkipDraw || this.Runs.Count is 0)
                return;

            for (int i = 0; i < Math.Min(this.Players.Children.Count, this.Rows - 1); i++)
            {
                Rectangle bounds = this.Players.Children[i].Bounds;
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
            this.HideButton = Attribute(node, "hide_button", false);
            this.IsSmall = Attribute(node, "small", false);
        }

        protected virtual void TryScroll(int rows)
        {
            int totalRuns = this.Board?.Runs.Count ?? 0;
            int maxOffset = Math.Max(totalRuns - this.Runs.Count, 0);
            this.ScrollOffset = MathHelper.Clamp(this.ScrollOffset + rows, 0, maxOffset);
        }

        private void UpdateScrollWheel()
        {
            int oldOffset = this.ScrollOffset;
            if (this.Players.Bounds.Contains(Input.Cursor(this.Root())))
            {
                if (Input.ScrolledUp())
                    this.ScrollUp(1);
                else if (Input.ScrolledDown())
                    this.ScrollDown(1);

                if (this.ScrollOffset != oldOffset)
                    this.Populate();
            }
        }
    }
}

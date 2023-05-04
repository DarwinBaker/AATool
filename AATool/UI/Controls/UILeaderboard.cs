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
        private readonly Dictionary<string, UIPersonalBest> runs;

        public const int MinimumRows = 6;

        public string Category { get; private set; }
        public string Version { get; private set; }
        public int Rows { get; private set; } = MinimumRows;

        private readonly Timer scrollWaitTimer = new (0.25, true);
        private readonly Timer scrollHoldTimer = new (0.05, true);

        private Leaderboard board;
        private UITextBlock title;
        private UIFlowPanel players;
        private UIFlowPanel menu;
        private UIPicture spinner;
        private UIPicture threeDots;

        private UIButton upButton;
        private UIPicture upIcon;

        private UIButton downButton;
        private UIPicture downIcon;

        private UIButton topButton;
        private UIPicture topIcon;

        private UIButton recenterButton;
        private UIPicture recenterOutline;
        private UIPicture recenterIcon;

        private UIButton refreshButton;
        private UIPicture refreshIcon;

        private string sourceSheet;
        private string sourcePage;
        private string mostRecords;
        private bool needsLayoutRefresh;
        private bool upToDate;
        private bool hideButton;
        private bool isSmall;
        private bool containsMainPlayer;
        private bool snapshot;
        private int mainPlayerOffset;
        private int scrollOffset;

        public UILeaderboard()
        {
            this.runs = new();
            this.BuildFromTemplate();
        }

        public bool LiveBoardAvailable => SpreadsheetRequest.DownloadedPages.Contains((this.sourceSheet, this.sourcePage));

        public void ScrollUp(int rows) => this.TryScroll(-rows);
        public void ScrollDown(int rows) => this.TryScroll(rows);

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);

            if (this.hideButton)
                this.First<UIGrid>()?.CollapseRow(0);

            this.upButton = this.First<UIButton>("up");
            this.upButton.OnClick += this.OnClick;
            this.upIcon = this.upButton?.First<UIPicture>();

            this.downButton = this.First<UIButton>("down");
            this.downButton.OnClick += this.OnClick;
            this.downIcon = this.downButton?.First<UIPicture>();

            this.topButton = this.First<UIButton>("top");
            this.topButton.OnClick += this.OnClick;
            this.topIcon = this.topButton?.First<UIPicture>();

            this.recenterButton = this.First<UIButton>("recenter");
            this.recenterButton.OnClick += this.OnClick;
            this.recenterOutline = this.recenterButton?.First<UIPicture>("recenter_outline");
            this.recenterIcon = this.recenterButton?.First<UIPicture>("recenter_avatar");

            this.refreshButton = this.First<UIButton>("refresh");
            this.refreshButton.OnClick += this.OnClick;
            this.refreshIcon = this.refreshButton?.First<UIPicture>();

            //this.recenterButton = this.First<UIButton>("recenter");
            //this.recenterButton.OnClick += this.OnClick;

            this.title = this.First<UITextBlock>("title");
            this.players = this.First<UIFlowPanel>("pb_list");
            this.menu = this.First<UIFlowPanel>("menu");
            this.spinner = this.First<UIPicture>("spinner");
            this.threeDots = this.First<UIPicture>("three_dots");

            //this.button.SetVisibility(UIMainScreen.ActiveTab == UIMainScreen.TrackerTab);
            //this.title.SetVisibility(UIMainScreen.ActiveTab != UIMainScreen.TrackerTab);

            if (this.isSmall)
                this.First<UIGrid>().SetRowHeight(0, new (36));

            this.UpdateTitle();
            
            //attempt to populate with cached data
            if (Leaderboard.TryGet(this.Category, this.Version, out Leaderboard board))
            {
                this.board = board;
                this.scrollOffset = this.mainPlayerOffset;
                this.Populate();
                if (UIMainScreen.ActiveTab == UIMainScreen.TrackerTab)
                    this.ScrollTo(Tracker.GetMainPlayer());
            }
            else
            {
                this.UpdateMenu();
            }

            string version = this.Version?.ToLower() ?? string.Empty;
            this.snapshot = version.Contains("snapshot") || version.Contains("w");
            if (this.snapshot)
            { 
                this.First<UIGrid>()?.RemoveControl(this.spinner);
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

        private void RequestRefresh()
        {
            if (this.Category is "All Blocks")
            {
                this.sourceSheet = Paths.Web.ABSheet;
                this.sourcePage = this.Version switch {
                    "1.16" => Paths.Web.ABPage16,
                    "1.18" => Paths.Web.ABPage18,
                    "1.19" => Paths.Web.ABPage19,
                    _ => string.Empty
                };
            }
            else
            {
                this.sourceSheet = Paths.Web.AASheet;
                this.sourcePage = this.Version is "1.16" ? Paths.Web.AAPage16 : Paths.Web.AAPageOthers;
            }

            if (!this.LiveBoardAvailable)
            {
                new SpreadsheetRequest($"{this.Version} {this.Category}", this.sourceSheet, this.sourcePage).EnqueueOnce();
                new SpreadsheetRequest("Player Nicknames", Paths.Web.NicknameSheet).EnqueueOnce();
            }
        }

        private void OnClick(UIControl sender)
        {
            int oldOffset = this.scrollOffset;
            if (sender == this.refreshButton)
            {
                Leaderboard.Refresh(this.Category, this.Version);
                this.RequestRefresh();
                UIMainScreen.ForceLayoutRefresh();
            }
            else if (sender == this.upButton)
            {
                this.ScrollUp(1);
            }
            else if (sender == this.downButton)
            {
                this.ScrollDown(1);
            }
            else if (sender == this.topButton)
            {
                this.scrollOffset = 0;
            }
            else if (sender == this.recenterButton)
            {
                this.scrollOffset = this.mainPlayerOffset;
            }
            this.needsLayoutRefresh = this.scrollOffset != oldOffset;
        }

        private void ScrollTo(Uuid mainPlayer)
        {
            string name;
            if (Player.NameCache.Any())
                Player.TryGetName(mainPlayer, out name);
            else
                name = Config.Tracking.LastPlayer;

            if (Leaderboard.TryGetRank(name, this.Category, this.Version, out int rank) && rank > 0)
            {
                this.recenterIcon.SetTexture($"avatar-{name.ToLower()}");
                int totalRuns = this.board?.Runs.Count ?? 0;
                int maxOffset = Math.Max(totalRuns - this.runs.Count, 0);
                this.mainPlayerOffset = MathHelper.Clamp(rank - 3, 0, maxOffset);
                this.scrollOffset = this.mainPlayerOffset;
                this.needsLayoutRefresh = true;
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

            if (this.isSmall)
            {
                string version = this.Version switch {
                    "1.6" => "1.0-1.6",
                    "1.11" => "1.8-1.11",
                    _ => this.Version,
                };
                this.title.SetText($"{version}");
                return;
            }

            string header = this.Version switch {
                "1.6" => "Unofficial 1.0-1.6\nAACH Leaderboard",
                "1.11" => "Unofficial 1.8-1.11\nAACH Leaderboard",
                _ => $"The Unofficial {this.Version}\n{acronym} Leaderboard",
            };
            this.title.SetText(header);
        }

        private void Clear()
        {
            this.players.Children.Clear();
            this.runs.Clear();
        }

        private void Populate()
        {
            if (this.Category is "All Blocks" && this.DrawMode is DrawMode.None)
            {
                this.PopuplateAllBlocks();
            }
            else if (UIMainScreen.ActiveTab is UIMainScreen.RunnersTab)
            {
                this.PopuplateSmall();
            }
            else
            { 
                if (!this.snapshot)
                    this.PopulateNormal();
                if (UIMainScreen.ActiveTab is UIMainScreen.MultiboardTab)
                    this.PopuplateMultiboard();
            }
            this.UpdateMenu();
            this.needsLayoutRefresh = false;
        }

        private void PopuplateAllBlocks()
        {
            if (this.board.Runs.Any())
            {
                Run wr = this.board.Runs[0];
                this.Root().First<UIAvatar>($"ab_wr_{this.Version}_avatar")?.SetPlayer(wr.Runner);
                this.Root().First<UIAvatar>($"ab_wr_{this.Version}_avatar")?.RegisterOnLeaderboard(this.board);
                this.Root().First<UIAvatar>($"ab_wr_{this.Version}_avatar")?.RefreshBadge();
                this.Root().First<UITextBlock>($"ab_wr_{this.Version}_runner")?.SetText(wr.Runner);
                this.Root().First<UITextBlock>($"ab_wr_{this.Version}_igt")?.SetText((int)wr.InGameTime.TotalHours + wr.InGameTime.ToString("':'mm':'ss"));
                this.Root().First<UITextBlock>($"ab_wr_{this.Version}_blocks")?.SetText($"({wr.Blocks} Blocks)");
            }
        }

        private void PopuplateMultiboard()
        {
            string mostRecordsList = string.Empty;
            for (int i = 0; i < Leaderboard.ListOfMostRecords.Count; i++)
            {
                mostRecordsList += Leaderboard.ListOfMostRecords[i].GameVersion;
                if (i < Leaderboard.ListOfMostRecords.Count - 1)
                    mostRecordsList += ", ";
            }
            //UIAvatar allBlocks20 = this.Root().First<UIAvatar>("ab_wr_1.20_avatar");
            //allBlocks20.SetBadge(new RankBadge(1, "All Blocks", "1.20", false));

            UIAvatar mostRecords = this.Root().First<UIAvatar>("most_records_avatar");
            mostRecords?.SetPlayer(Leaderboard.RunnerWithMostWorldRecords);
            this.Root().First<UITextBlock>("most_records_runner")?.SetText(Leaderboard.RunnerWithMostWorldRecords);
            this.Root().First<UITextBlock>("most_records_list")?.SetText(mostRecordsList);

            if (!string.IsNullOrEmpty(Leaderboard.RsgRunner))
            {
                this.Root().First<UIAvatar>("any%_rsg_avatar")?.SetPlayer(Leaderboard.RsgRunner);
                this.Root().First<UITextBlock>("any%_rsg_runner")?.SetText(Leaderboard.RsgRunner);
                this.Root().First<UITextBlock>("any%_rsg_time")?.SetText($"{Leaderboard.RsgInGameTime:m':'ss} IGT      {Leaderboard.RsgRealTime:m':'ss} RTA");
            }
            if (!string.IsNullOrEmpty(Leaderboard.SsgRunner))
            {
                this.Root().First<UIAvatar>("any%_ssg_avatar")?.SetPlayer(Leaderboard.SsgRunner);
                this.Root().First<UITextBlock>("any%_ssg_runner")?.SetText(Leaderboard.SsgRunner);
                this.Root().First<UITextBlock>("any%_ssg_time")?.SetText($"{Leaderboard.SsgInGameTime:m':'ss} IGT      {Leaderboard.SsgRealTime:m':'ss} RTA");
            }
        }

        private void PopuplateSmall()
        {
            this.Clear();
            UIFlowPanel list = this.Root().First<UIFlowPanel>("runner_list");
            if (list is not null && list.Children.Count is 0)
            {
                list.ClearControls();
                for (int i = 0; i < this.board.Runs.Count; i++)
                {
                    var control = new UIPersonalBest(this.board) { FlexWidth = new (150), IsSmall = true };
                    Run run = this.board.Runs[i + this.scrollOffset];
                    this.runs[run.Runner] = control;
                    new AvatarRequest(Leaderboard.GetRealName(run.Runner)).EnqueueOnce();

                    control.SetRun(run);
                    control.InitializeRecursive(this.Root());
                    list.AddControl(control);
                }
            }
            list.ReflowChildren();
        }

        private void PopulateNormal()
        {
            this.Clear();
            this.containsMainPlayer = false;
            Run run = null;
            for (int i = 0; i < this.Rows; i++)
            {
                bool claimed = i < this.board.Runs.Count;
                var control = new UIPersonalBest(this.board) { IsSmall = this.isSmall, Rank = i + this.scrollOffset + 1 };
                if (claimed)
                {
                    run = this.board.Runs[i + this.scrollOffset];
                    this.runs[run.Runner] = control;
                    new AvatarRequest(Leaderboard.GetRealName(run.Runner)).EnqueueOnce();
                }
                else
                {
                    control.SetRank(i + 1);
                }
                control.SetRun(run, claimed);
                control.InitializeRecursive(this.Root());
                this.players.AddControl(control);
                this.containsMainPlayer |= control.IsMainPlayer;
            }
            this.players.ResizeRecursive(this.hideButton ? this.Bounds : this.players.Bounds);
            this.First<UIGrid>()?.RemoveControl(this.spinner);
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.players.ResizeRecursive(this.hideButton ? this.Bounds : this.players.Bounds);
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.upToDate && this.LiveBoardAvailable)
            {
                if (Leaderboard.TryGet(this.Category, this.Version, out Leaderboard live))
                {
                    this.board = live;
                    this.Populate();
                    this.UpdateMenu();
                }  
                this.upToDate = true;
            }

            if (Config.Tracking.FilterChanged)
                UIMainScreen.Invalidate();

            Uuid mainPlayer = Tracker.GetMainPlayer();
            if (Tracker.MainPlayerChanged)
                this.ScrollTo(mainPlayer);

            if (this.needsLayoutRefresh)
                this.Populate();

            if (Config.Main.AppearanceChanged)
                this.UpdateMenu();

            this.UpdateScrollWheel();
            this.UpdateHoldScroll(time);
        }

        private void UpdateMenu()
        {
            bool available = Leaderboard.IsLiveAvailable(this.Category, this.Version);
            this.threeDots.SetVisibility(!this.snapshot && !available);

            bool menuChanged = false;
            menuChanged |= this.topButton.IsCollapsed == (this.scrollOffset > 0);
            menuChanged |= this.refreshButton.IsCollapsed == available;
            menuChanged |= this.recenterButton.IsCollapsed == (this.scrollOffset != this.mainPlayerOffset);

            int runs = this.board?.Runs.Count ?? 0;
            this.upButton.SetVisibility(this.Rows < runs);
            this.downButton.SetVisibility(this.Rows < runs);
            this.topButton.SetVisibility(this.Rows < runs);
            this.recenterButton.SetVisibility(this.Rows < runs);

            this.upButton.Enabled = this.scrollOffset > 0;
            this.topButton.Enabled = this.scrollOffset > 0;

            int totalRuns = this.board?.Runs.Count ?? 0;
            int maxOffset = Math.Max(totalRuns - this.runs.Count, 0);
            this.downButton.Enabled = this.scrollOffset < maxOffset;

            this.refreshButton.Enabled = available;
            this.recenterButton.SetVisibility(!this.containsMainPlayer && !string.IsNullOrEmpty(this.recenterIcon.Texture));

            Color dim = ColorHelper.Fade(Config.Main.TextColor, 0.1f);
            this.upIcon.SetTint(this.upButton.Enabled ? Config.Main.TextColor : dim);
            this.downIcon.SetTint(this.downButton.Enabled ? Config.Main.TextColor : dim);
            this.topIcon.SetTint(this.topButton.Enabled ? Config.Main.TextColor : dim);
            this.refreshIcon.SetTint(this.refreshButton.Enabled ? Config.Main.TextColor : dim);
            this.recenterOutline.SetTint(Config.Main.TextColor);

            if (menuChanged)
                this.menu.ReflowChildren();
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
                    int oldOffset = this.scrollOffset;
                    if (this.upButton.Bounds.Contains(cursor))
                    {
                        this.ScrollUp(1);
                        this.scrollHoldTimer.Reset();
                    }
                    else if (this.downButton.Bounds.Contains(cursor))
                    {
                        this.ScrollDown(1);
                        this.scrollHoldTimer.Reset();
                    }
                    this.needsLayoutRefresh = this.scrollOffset != oldOffset;
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

            if (this.SkipDraw || this.runs.Count is 0)
                return;

            for (int i = 0; i < Math.Min(this.players.Children.Count, this.Rows - 1); i++)
            {
                Rectangle bounds = this.players.Children[i].Bounds;
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
            this.hideButton = Attribute(node, "hide_button", false);
            this.isSmall = Attribute(node, "small", false);
        }

        private void TryScroll(int rows)
        {
            int totalRuns = this.board?.Runs.Count ?? 0;
            int maxOffset = Math.Max(totalRuns - this.runs.Count, 0);
            this.scrollOffset = MathHelper.Clamp(this.scrollOffset + rows, 0, maxOffset);
        }

        private void UpdateScrollWheel()
        {
            int oldOffset = this.scrollOffset;
            if (this.players.Bounds.Contains(Input.Cursor(this.Root())))
            {
                if (Input.ScrolledUp())
                    this.ScrollUp(1);
                else if (Input.ScrolledDown())
                    this.ScrollDown(1);

                if (this.scrollOffset != oldOffset)
                    this.Populate();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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
    internal class UIRunnerProfile : UIPanel
    {
        private readonly Timer scrollWaitTimer = new(0.25, true);
        private readonly Timer scrollHoldTimer = new(0.05, true);

        private List<(Leaderboard board, Run run)> runs = new();

        float srcProfileAlpha = 0;

        RunnerProfile profile;
        string minecraftAvatar;
        bool hasProfilePicture;

        UIPicture avatar;
        UITextBlock nameLabel;
        UITextBlock pronounsLabel;
        UITextBlock showingLabel;
        UIFlowPanel trophyPanel;
        UITextInput runnerSearch;

        UIButton upButton;
        UIButton downButton;

        bool needsLayoutRefresh;
        int scrollOffset;
        int maxRows = 9;

        public UIRunnerProfile()
        {
            this.BuildFromTemplate();
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.TryGetFirst(out this.avatar, "avatar");
            this.TryGetFirst(out this.nameLabel, "name");
            this.TryGetFirst(out this.pronounsLabel, "pronouns");
            this.TryGetFirst(out this.showingLabel, "showing");
            this.TryGetFirst(out this.trophyPanel, "trophies");

            if (this.Root().TryGetFirst(out this.runnerSearch, "runner_search"))
            {
                if (!string.IsNullOrEmpty(Config.Tracking.CurrentRunnerProfileName))
                {
                    this.runnerSearch.SetText(Config.Tracking.CurrentRunnerProfileName);
                }
                else if (RunnerProfile.NamesBySrcId.TryGetValue(Config.Tracking.CurrentRunnerProfileId, out string name))
                {
                    this.runnerSearch.SetText(name);
                }

                this.runnerSearch.MaxLength = 20;
                this.runnerSearch.GoToEnd();
                this.runnerSearch.OnTextChanged += this.RunnerSearchChanged;
            }

            if (this.TryGetFirst(out this.upButton, "up"))
                this.upButton.OnClick += this.OnClick;

            if (this.TryGetFirst(out this.downButton, "down"))
                this.downButton.OnClick += this.OnClick;

            this.Root().First<UITextBlock>("runner_name")?.SetText(Config.Tracking.CurrentRunnerProfileName);
            this.pronounsLabel?.SetText(string.Empty);
        }

        private void RunnerSearchChanged(UIControl sender)
        {
            RunnerProfile.SetCurrentName(this.runnerSearch.UserInput);

            if (!string.IsNullOrEmpty(this.runnerSearch.UserInput))
                this.Root().First<UIRecordGraph>()?.UpdateRunner();
        }

        public void ScrollUp(int rows) => this.TryScroll(-rows);
        public void ScrollDown(int rows) => this.TryScroll(rows);

        protected virtual void TryScroll(int rows)
        {
            int maxOffset = Math.Max(this.runs.Count - this.maxRows, 0);
            this.scrollOffset = MathHelper.Clamp(this.scrollOffset + rows, 0, maxOffset);
            this.UpdateMenu();
        }

        private void OnClick(UIControl sender)
        {
            int oldOffset = this.scrollOffset;
            if (sender == this.upButton)
            {
                this.ScrollUp(1);
            }
            else if (sender == this.downButton)
            {
                this.ScrollDown(1);
            }
            this.needsLayoutRefresh = this.scrollOffset != oldOffset;
        }

        public override void ResizeThis(Rectangle parent)
        {
            base.ResizeThis(parent);

            if (Config.Tracking.CurrentRunnerProfileName.Value.ToLower() == "couriway")
            {
                InitializeHundredThousandRuns();
            }
            else
            {
                var bottomBar = new UIPanel() {
                    FlexHeight = new Size(8, SizeMode.Absolute),
                    VerticalAlign = VerticalAlign.Bottom,
                    BorderThickness = 8,
                    InnerCorners = false,
                };
                this.AddControl(bottomBar);
            }

        }

        private void InitializeHundredThousandRuns()
        {
            this.trophyPanel.Padding = new Margin(-20, 0, 0, 0);
            this.runnerSearch.Enabled = false;

            if (this.Root().TryGetFirst(out UIHundredThousandGrid grid))
            {
                grid.DrawMode = DrawMode.All;
                var dimensionsLabel = new UITextBlock() {
                    VerticalTextAlign = VerticalAlign.Bottom,
                    Padding = new Margin(0, 0, 0, 15),
                };
                dimensionsLabel.SetText("(grid is 500 wide x 200 tall)");
                this.AddControl(dimensionsLabel);

                var bottomBar = new UIPanel() {
                    FlexHeight = new Size(5, SizeMode.Absolute),
                    VerticalAlign = VerticalAlign.Bottom,
                    BorderThickness = 0,
                    InnerCorners = false,
                };
                this.AddControl(bottomBar);

                if (Leaderboard.AllBoards.TryGetValue(("1K No Reset", "1.16"), out Leaderboard board)
                    && board.Runs.FirstOrDefault() is Run run)
                {
                    grid.SetData(run);

                }
            }

            this.Root().First<UIRecordGraph>().DrawMode = DrawMode.None;
            this.Root().First<UITextBlock>("runner_graph_title")?.SetText("100K Speedruns Challenge");
        }

        private void UpdateRuns()
        {
            this.runs.Clear();

            foreach (Leaderboard board in Leaderboard.AllBoards.Values)
            {
                if (board is not null)
                    this.CheckForCompletedRun(board);
            }

            this.SortRuns();
            this.CheckForSpecial();
            this.UpdateMenu();
            this.PopulateTrophies();
        }

        private void CheckForCompletedRun(Leaderboard board)
        {
            string name = this.profile?.Name ?? Config.Tracking.CurrentRunnerProfileName;

            foreach (Run run in board.Runs)
            {
                if (string.Equals(run.Runner, name, StringComparison.OrdinalIgnoreCase))
                {
                    this.runs.Add((board, run));
                    break;
                }
            }
        }

        private void CheckForSpecial()
        {
            string name = this.profile?.Name ?? Config.Tracking.CurrentRunnerProfileName;

            //aa ssg
            if (string.Equals(name, Leaderboard.AASsgRunner, StringComparison.OrdinalIgnoreCase))
            {
                var board = new Leaderboard("AA SSG", "1.16");
                var run = new Run(name, Leaderboard.AASsgInGameTime) {
                    Comment = "AA SSG WR"
                };
                board.AddRun(run, 1);
                this.runs.Insert(0, (board, run));
            }

            //half heart hardcore aa
            if (Leaderboard.HalfHeartHardcoreCompletions.Runs.FirstOrDefault() is Run hhh)
            {
                if (string.Equals(name, hhh.Runner, StringComparison.OrdinalIgnoreCase))
                {
                    this.runs.Insert(0, (Leaderboard.HalfHeartHardcoreCompletions, hhh));
                }
            }

            //concurrent world records
            if (string.Equals(name, Leaderboard.RunnerWithMostConcurrentRecords, StringComparison.OrdinalIgnoreCase))
            {
                var board = new Leaderboard("Most Records", "Concurrent");
                var run = new Run(name, default) {
                    Comment = "Most Concurrent Records"
                };
                board.AddRun(run, 1);
                this.runs.Insert(0, (board, run));
            }

            //consecutive world records
            if (string.Equals(name, Leaderboard.RunnerWithMostConsecutiveRecords, StringComparison.OrdinalIgnoreCase))
            {
                var board = new Leaderboard("Most Records", "Consecutive");
                var run = new Run(name, default) {
                    Comment = "Most Consecutive Records"
                };
                board.AddRun(run, 1);
                this.runs.Insert(0, (board, run));
            }
        }

        private void SortRuns()
        {
            //move challenges to top first
            for (int i = 0; i < this.runs.Count; i++)
            {
                (Leaderboard board, Run run) item = this.runs[i];
                if (item.run is HardcoreStreak || item.run.Comment == "1K No Reset")
                {
                    this.runs.Remove(item);
                    this.runs.Insert(0, item);
                }
            }

            //sort by ranks, then by game versions
            for (int i = 0; i < this.runs.Count - 1; i++)
            {
                for (int j = 0; j < this.runs.Count - i - 1; j++)
                {
                    (Leaderboard board, Run run) a = this.runs[j];
                    (Leaderboard board, Run run) b = this.runs[j + 1];

                    a.board.Ranks.TryGetValue(a.run.Runner.ToLower(), out int rankA);
                    b.board.Ranks.TryGetValue(b.run.Runner.ToLower(), out int rankB);

                    if (rankA > rankB || (rankA == rankB && ShouldSwapByCategoryAndVersion(a, b)))
                    {
                        this.runs[j] = b;
                        this.runs[j + 1] = a;
                    }
                }
            }
        }

        private static bool ShouldSwapByCategoryAndVersion((Leaderboard board, Run run) a, (Leaderboard board, Run run) b)
        {
            if (a.board.Category != b.board.Category)
                return false;

            if (a.run.GameVersion is null || b.run.GameVersion is null)
                return false;

            return a.run.GameVersion < b.run.GameVersion;
        }

        private void PopulateTrophies()
        {
            if (this.trophyPanel is null)
                return;

            this.trophyPanel.ClearControls();

            int rows = Math.Min(this.runs.Count, this.maxRows);

            for (int i = 0; i < rows; i++)
            {
                Leaderboard board = this.runs[i + this.scrollOffset].board;
                Run run = this.runs[i + this.scrollOffset].run;

                new AvatarRequest(run.Runner).EnqueueOnce();

                var pb = new UIPersonalBest(board) {
                    FlexWidth = new Size(210),
                    FlexHeight = new Size(70),
                    HorizontalAlign = HorizontalAlign.Left,
                    Margin = new Margin(27, 0, 0, 0),
                    DisableButton = true,
                };

                if (run is HardcoreStreak streak)
                {
                    pb.NameOverride = "Hardcore Challenge";
                    pb.TimeOverride = $"{streak.BestStreak} Deathless Streak";
                }
                else if (run is AllVersionsRun avRun)
                {
                    pb.NameOverride = $"All Versions ({avRun.Range})";
                }
                else if (run.Comment == "Most Concurrent Records")
                {
                    pb.NameOverride = run.Comment;
                    pb.TimeOverride = $"WRs in {Leaderboard.ListOfMostConcurrentRecords.Count} AA versions";
                }
                else if (run.Comment == "Most Consecutive Records")
                {
                    pb.NameOverride = "Most Consecutive AA WRs";
                    pb.TimeOverride = $"{Leaderboard.MostConsecutiveRecordsCount} back-to-back (1.16)";
                }
                else if (board.Category.Contains("1K"))
                {
                    pb.NameOverride = $"No-Reset Any% ({board.Version})";
                }

                else if (board.Category.Contains("HHH"))
                {
                    pb.NameOverride = $"Half-Heart Hardcore AA WR";
                    pb.TimeOverride = $"{run.InGameTime} ({board.Version})";
                }
                else
                {
                    pb.NameOverride = $"{board.Category} ({board.Version})";
                }

                pb.IsSmall = true;
                pb.SetRun(run);

                var line = new UIPanel() {
                    MaxWidth = new Size(176),
                    FlexHeight = new Size(2),
                    VerticalAlign = VerticalAlign.Top,
                    HorizontalAlign = HorizontalAlign.Left,
                    Margin = new Margin(5, 0, 48, 0),
                    InnerCorners = false,
                };
                pb.AddControl(line);

                this.trophyPanel.AddControl(pb);
                pb.InitializeRecursive(this.Root());
            }

            this.trophyPanel.ResizeRecursive(this.Inner);
            UIMainScreen.Invalidate();
        }

        protected override void UpdateThis(Time time)
        {
            base.UpdateThis(time);

            if (this.needsLayoutRefresh)
            {
                this.PopulateTrophies();
                this.needsLayoutRefresh = false;
            }

            if (RunnerProfile.GetCurrent() is RunnerProfile current)
            {
                bool identityChanged = current.Id != this.profile?.Id || current.Name != this.profile?.Name;
                bool pictureChanged = current.Picture is not null != this.hasProfilePicture;

                if (identityChanged)
                {
                    this.Root().First<UITextBlock>("runner_name")?.SetText(current.Name);
                    this.pronounsLabel?.SetText(current.Pronouns);
                }

                if (identityChanged || pictureChanged)
                    this.profile = current;

                if (identityChanged && this.profile.Picture is not null)
                    this.srcProfileAlpha = 1;

                if (identityChanged)
                {
                    this.UpdateRuns();
                }
            }
            
            if (this.profile is not null)
                this.minecraftAvatar = $"avatar-{Leaderboard.GetRealName(this.profile.Name ?? string.Empty).ToLower()}";

            if (this.profile?.Picture is not null)
            {
                this.srcProfileAlpha = MathHelper.Lerp(this.srcProfileAlpha, 1, 2f * (float)time.Delta);
            }
            else
            {
                this.srcProfileAlpha = 0;
            }

            this.UpdateScrollWheel();
            this.UpdateHoldScroll(time);
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

        private void UpdateScrollWheel()
        {
            int oldOffset = this.scrollOffset;
            if (this.trophyPanel.Bounds.Contains(Input.Cursor(this.Root())))
            {
                if (Input.ScrolledUp())
                    this.ScrollUp(1);
                else if (Input.ScrolledDown())
                    this.ScrollDown(1);

                if (this.scrollOffset != oldOffset)
                {
                    this.needsLayoutRefresh = true;
                }
            }
        }

        private void UpdateMenu()
        {
            int runs = this.runs.Count;
            this.upButton.SetVisibility(this.maxRows < runs);
            this.downButton.SetVisibility(this.maxRows < runs);

            this.upButton.Enabled = this.scrollOffset > 0;

            int maxOffset = Math.Max(runs - this.maxRows, 0);
            this.downButton.Enabled = this.scrollOffset < maxOffset;

            if (this.runs.Count > this.maxRows)
            {
                int start = this.scrollOffset + 1;
                int end = this.scrollOffset + this.maxRows;
                int total = this.runs.Count;
                this.showingLabel?.SetText($"Showing {start}-{end} of {total}");
            }
            else
            {
                this.showingLabel?.SetText(string.Empty);
            }
        }

        /*
        public override void DrawThis(Canvas canvas)
        {
            base.DrawThis(canvas);

            if (this.profile is null)
                return;

            canvas.Draw(this.minecraftAvatar, this.avatar.Bounds, Color.White, Layer.Fore);

            if (this.profile.Picture is not null)
                canvas.Draw(this.profile.Picture, this.avatar.Bounds, Color.White * this.srcProfileAlpha, Layer.Fore);
        }
        */
    }
}

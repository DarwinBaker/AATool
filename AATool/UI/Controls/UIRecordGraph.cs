using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Screens;
using AATool.Utilities;
using FontStashSharp;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIRecordGraph : UIPanel
    {
        public string Category { get; private set; }
        public string Version { get; private set; }

        private bool upToDate;
        private bool runnerMode;
        private bool runnerChanged;
        private bool sliderInitialized;

        RunnerProfile runnerProfile = RunnerProfile.GetCurrent();

        public bool LiveHistoryAvailable => SpreadsheetRequest.DownloadedPages.Contains(
            (Paths.Web.AASheet, Paths.Web.PrimaryAAHistory));

        readonly List<Run> records = new();
        readonly Dictionary<string, TimeSpan> timeHeldByRunner = new();
        readonly List<(Run, Rectangle)> avatars = new();
        readonly Dictionary<Rectangle, Run> runHoverBounds = new();

        string singlePlayer;

        UIControl hoverPanel;
        UIControl graph;

        UISlider timeSlider;
        UISlider dateSlider;

        UITextBlock noRecordsLabel;

        DynamicSpriteFont font;

        float maxHours = float.MinValue;
        float targetMaxHours = float.MinValue;

        Run firstPlace;
        Run lastPlace;

        double newestDateSeconds;
        double oldestDateSeconds;
        //double maxDateSeconds;
        //double targetMaxDateSeconds = double.MinValue;

        float fastestTotalSeconds;
        float slowestTotalSeconds;

        int fastestHours;
        int slowestHours;

        bool hhhMode = false;

        Rectangle hoveredBounds = Rectangle.Empty;
        float hoverAlpha = 0;
        float hoverX = float.MinValue;
        Run hoveredRun;

        public UIRecordGraph()
        {
            this.BuildFromTemplate();
        }

        internal void UpdateRunner()
        {
            var newProfile = RunnerProfile.GetCurrent();
            if (this.runnerProfile != newProfile)
            {
                this.runnerProfile = newProfile;
                this.runnerChanged = true;
            }
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);

            //attempt to populate with cached or already downloaded data
            if (Leaderboard.History is not null)
                this.Populate(Leaderboard.History);

            if (!this.LiveHistoryAvailable)
                new SpreadsheetRequest("history_aa_1.16", Paths.Web.AASheet, Paths.Web.PrimaryAAHistory).EnqueueOnce();
        }

        public override void InitializeThis(UIScreen screen)
        {
            base.InitializeThis(screen);
            this.font = FontSet.Get("minecraft", 12);
            this.graph = this.First("graph");
            if (this.TryGetFirst(out this.hoverPanel, "hover_test"))
            {
                if (this.hoverPanel.TryGetFirst(out UIButton profileButton, "profile"))
                    profileButton.OnClick += this.ProfileButtonClick;

                if (this.hoverPanel.TryGetFirst(out UIButton linkButton, "link"))
                    linkButton.OnClick += this.LinkButtonClick;
            }

            if (this.TryGetFirst(out this.timeSlider, "time_slider"))
            { 
                this.timeSlider.OnValueChange += this.TimeSliderValueChange;
                this.timeSlider.Value = 0.7f;
            }

            this.TryGetFirst(out this.dateSlider, "date_slider");
            this.TryGetFirst(out this.noRecordsLabel, "no_records");
        }

        /*
        private void DateSliderValueChange(UISlider sender)
        {
            double todaySeconds = TimeSpan.FromTicks(DateTime.Today.Ticks).TotalSeconds;
            double maxDateSecondsTargetPrevious = this.targetMaxDateSeconds;
            double range = Math.Abs(todaySeconds - this.oldestDateSeconds);
            this.targetMaxDateSeconds = ((1 - sender.Value) * range) + this.oldestDateSeconds;
            this.targetMaxDateSeconds = Math.Max(this.targetMaxDateSeconds, this.oldestDateSeconds);
            this.targetMaxDateSeconds = Math.Min(this.targetMaxDateSeconds, this.newestDateSeconds);

            if (this.targetMaxDateSeconds != maxDateSecondsTargetPrevious)
            {
                UIMainScreen.Invalidate();
                this.UpdateVariables();
                this.UpdateAvatars();
            }
        }
        */

        private void TimeSliderValueChange(UISlider sender)
        {
            float min = 2;
            if (this.records.Count > 1)
            {
                double previousRecordHours = this.records[this.records.Count - 2].InGameTime.TotalHours;
                double currentRecordHours = this.records.Last().InGameTime.TotalHours;
                min = (float)(previousRecordHours + (previousRecordHours - currentRecordHours));
            }

            float max = 35;
            if (this.records.Count > 0)
            {
                double firstRecordHours = this.records.First().InGameTime.TotalHours;
                max = (int)Math.Round(firstRecordHours, MidpointRounding.AwayFromZero);
            }

            float maxHoursTargetPrevious = this.targetMaxHours;
            float range = max - min;
            this.targetMaxHours = (float)(sender.Value * range) + min;
            this.targetMaxHours = Math.Max(this.targetMaxHours, min);
            this.targetMaxHours = Math.Min(this.targetMaxHours, max);

            if (this.targetMaxHours != maxHoursTargetPrevious)
            {
                UIMainScreen.Invalidate();
                this.UpdateVariables();
                this.UpdateAvatars();
            }
        }

        private void ProfileButtonClick(UIControl sender)
        {
            if (this.hoveredRun is not null)
            {
                RunnerProfile.SetCurrentName(this.hoveredRun.Runner);
                UIMainScreen.SetActiveTab(UIMainScreen.RunnerProfileTab);
            }
        }

        private void LinkButtonClick(UIControl sender)
        {
            if (!string.IsNullOrEmpty(this.hoveredRun?.Link))
                Process.Start(this.hoveredRun?.Link);
        }

        private bool FilterAllows(Run run)
        {
            if (this.hhhMode)
            {
                return run.Comment.Contains("HHH");
            }
            else if (this.runnerMode)
            {
                return this.runnerProfile?.Name?.ToLower() == run.Runner.ToLower();
            }

            return true;
        }

        private void Populate(LeaderboardSheet history)
        {
            this.records.Clear();
            string test = string.Empty;
            TimeSpan wr = TimeSpan.MaxValue;
            for (int row = 1; row < history.Rows.Length; row++)
            {
                if (!Run.TryParse(history, row, "1.16", out Run run))
                    continue;

                if (!string.IsNullOrEmpty(this.singlePlayer) && run.Runner.ToLower() != this.singlePlayer)
                    continue;

                if (run.InGameTime <= wr && this.FilterAllows(run))
                {
                    wr = run.InGameTime;
                    this.records.Add(run);
                    test += $"{run.Runner} - {wr}\n";
                }
            }

            if (!this.records.Any())
            {
                this.ShowNoRecordsView();
                return;
            }
            else
            {
                this.HideNoRecordsView();
            }

            string dates = "";
            string previousHolder = this.records.First().Runner;
            DateTime holdStart = this.records.First().Date;
            for (int i = 0; i < this.records.Count; i++)
            {
                Run run = this.records[i];
                if (run.Runner != previousHolder || i == this.records.Count - 1)
                {
                    this.timeHeldByRunner.TryGetValue(previousHolder, out TimeSpan totalTimeHeld);
                    totalTimeHeld = totalTimeHeld.Add(new TimeSpan((run.Date - holdStart).Ticks));
                    this.timeHeldByRunner[previousHolder] = totalTimeHeld;

                    previousHolder = run.Runner;
                    holdStart = run.Date;
                }
                dates += $"{run.Date} - {run.Runner}\n";
            }

            if (this.targetMaxHours is float.MinValue)
            {
                this.targetMaxHours = (int)Math.Round(this.records.First().InGameTime.TotalHours, MidpointRounding.AwayFromZero);
                this.targetMaxHours = Math.Min(this.targetMaxHours, 26);
                
                if (this.hhhMode || this.runnerMode)
                    this.timeSlider.Value = 1;
            }
        }

        private void ShowNoRecordsView()
        {
            this.noRecordsLabel?.SetVisibility(true);

            if (this.timeSlider is not null)
            {
                this.timeSlider.Enabled = false;
                this.timeSlider.Value = 0;
            }

            if (this.dateSlider is not null)
            {
                this.dateSlider.Enabled = false;
                this.dateSlider.Value = 0;
            }

            if (this.runnerMode)
            {
                string message = string.IsNullOrEmpty(this.runnerProfile?.Name)
                    ? "No player selected"
                    : $"{this.runnerProfile.Name} has no 1.16 AA completions";

                this.noRecordsLabel?.SetText(message);
            }
            else
            {
                this.noRecordsLabel?.SetText("No Records Found");
            }
        }

        private void HideNoRecordsView()
        {
            this.noRecordsLabel?.SetVisibility(false);

            if (this.timeSlider is not null)
            {
                this.timeSlider.Enabled = true;
            }

            if (this.dateSlider is not null)
            {
                this.dateSlider.Enabled = true;
            }

            this.noRecordsLabel?.SetText(string.Empty);
        }

        protected override void UpdateThis(Time time)
        {
            if (this.runnerChanged || (!this.upToDate && this.LiveHistoryAvailable))
            {
                this.Populate(Leaderboard.History);
                this.upToDate = true;

                if (this.runnerChanged)
                {
                    this.sliderInitialized = false;
                }
            }

            this.UpdateInput(time);

            float maxHoursPrevious = this.maxHours;
            this.maxHours = this.maxHours is float.MinValue
                ? this.targetMaxHours
                : MathHelper.Lerp(this.maxHours, this.targetMaxHours, (float)(20 * time.Delta));

            //double maxDateSecondsPrevious = this.maxDateSeconds;
            //this.maxDateSeconds = this.maxDateSeconds is double.MinValue
            //    ? this.targetMaxHours
            //    : this.maxDateSeconds + ((this.targetMaxDateSeconds - this.maxDateSeconds) * (float)(20 * time.Delta));
            
            if (!this.records.Any())
            {
                this.hoverPanel?.SetVisibility(false);
                return;
            }

            if (this.maxHours != maxHoursPrevious)// || this.maxDateSeconds != maxDateSecondsPrevious)
            {
                UIMainScreen.Invalidate();
                this.UpdateVariables();
                this.UpdateAvatars();
            }

            //if (this.targetMaxDateSeconds is double.MinValue)
            //    this.dateSlider.Value = 1;

            this.UpdateHover(time);

            if (!this.sliderInitialized)
            {
                if (this.hhhMode || this.runnerMode)
                {
                    this.timeSlider.Value = 1;
                }
                this.sliderInitialized = true;
            }
        }

        void UpdateInput(Time time)
        {
            if (Input.IsDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                double speed = 0.25 + (this.timeSlider.Value * (this.timeSlider.Value / 2f));
                this.timeSlider.Value += speed * time.Delta;
            }

            if (Input.IsDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                double speed = 0.25 + (this.timeSlider.Value * (this.timeSlider.Value / 2f));
                this.timeSlider.Value -= speed * time.Delta;
            }

            if (this.Bounds.Contains(Input.Cursor(this.Root())))
            {
                if (Input.ScrolledDown())
                    this.timeSlider.Value += 0.005f + (this.timeSlider.Value / 10);
                else if (Input.ScrolledUp())
                    this.timeSlider.Value -= 0.005f + (this.timeSlider.Value / 10);
            }
        }

        void UpdateHover(Time time)
        {
            Point cursor = Input.Cursor(this.Root());

            Rectangle previousHoveredBounds = this.hoveredBounds;

            if (!this.hoverPanel.Bounds.Contains(cursor))
            {
                this.hoveredBounds = Rectangle.Empty;
                int index = 0;
                foreach (KeyValuePair<Rectangle, Run> run in this.runHoverBounds)
                {
                    if (run.Key.Contains(cursor))
                    {
                        this.hoverPanel?.SetVisibility(true);
                        UIAvatar avatar = this.hoverPanel?.First<UIAvatar>();
                        avatar?.SetPlayer(run.Value.Runner);

                        if (this.hhhMode)
                            avatar?.SetBadge(new HalfHeartHardcoreBadge());
                        else if (this.runnerMode)
                            avatar?.SetBadge(null);
                        else
                            avatar?.SetBadge(new RankBadge(1, "All Advancements", "1.16"));

                        string type = this.runnerMode ? "PB" : "WR";

                        this.hoverPanel?.First<UITextBlock>("runner")?.SetText(run.Value.Runner);
                        this.hoverPanel?.First<UITextBlock>("igt")?.SetText($"{(int)run.Value.InGameTime.TotalHours}:{run.Value.InGameTime:mm':'ss} IGT");
                        if (index == this.runHoverBounds.Count - 1)
                            this.hoverPanel?.First<UITextBlock>("wr_number")?.SetText($"Current {type}");
                        else
                            this.hoverPanel?.First<UITextBlock>("wr_number")?.SetText($"Former {type} #{index + 1}");

                        this.hoverPanel?.First<UITextBlock>("set")?.SetText($"Set on {run.Value.Date:MMM d, yyyy}");
                        if (index == this.runHoverBounds.Count - 1)
                        {
                            int totalDays = (int)(DateTime.UtcNow - run.Value.Date).TotalDays;
                            string daysStanding = totalDays is 1 ? $"{totalDays} day" : $"{totalDays} days";
                            this.hoverPanel?.First<UITextBlock>("lasted")?.SetText($"({daysStanding} ago)");
                            this.hoverPanel?.First<UITextBlock>("beaten")?.SetText("");
                        }
                        else
                        {
                            Run nextRun = this.records[index + 1];
                            int totalDays = (int)(nextRun.Date - run.Value.Date).TotalDays;
                            string daysStanding = totalDays is 1 ? $"{totalDays} day" : $"{totalDays} days";
                            this.hoverPanel?.First<UITextBlock>("lasted")?.SetText($"Stood for {daysStanding}");

                            if (!this.runnerMode)
                            {
                                this.hoverPanel?.First<UITextBlock>("beaten")?.SetText($"Beaten by {nextRun.Runner}");
                            }
                        }

                        this.hoverPanel.First<UIButton>("link")?.SetVisibility(!string.IsNullOrEmpty(run.Value.Link));

                        this.hoveredRun = run.Value;
                        this.hoveredBounds = run.Key;
                        break;
                    }
                    index++;
                }
            }
            
            if (this.hoveredBounds == Rectangle.Empty)
            {
                this.hoverX = float.MinValue;
                this.hoveredRun = null;
                this.hoverPanel?.SetVisibility(false);
            }
            else
            {
                float targetX = this.hoveredBounds.Right + this.hoverPanel.Width < this.Right
                    ? this.hoveredBounds.Right
                    : this.hoveredBounds.Left - this.hoverPanel.Width;

                if (this.hoverX is float.MinValue)
                    this.hoverX = targetX;

                this.hoverX = MathHelper.Lerp(this.hoverX, targetX, 20 * (float)time.Delta);
                if ((int)this.hoverX != this.hoverPanel.X)
                {
                    this.hoverPanel.MoveTo(new Point((int)this.hoverX, this.graph.Top + 24));
                }

                if (Math.Abs(this.hoverX - targetX) < 0.1)
                    this.hoverX = targetX;
            }

            if (this.hoveredBounds != previousHoveredBounds)
            {
                this.hoverAlpha = 0;
                UIMainScreen.Invalidate();
            }

            float targetAlpha = this.hoveredBounds != Rectangle.Empty ? 0.15f : 0;
            this.hoverAlpha = MathHelper.Lerp(this.hoverAlpha, targetAlpha, 3 * (float)time.Delta);

            if (this.hoverAlpha != targetAlpha)
                UIMainScreen.Invalidate();

            if (Math.Abs(this.hoverAlpha - targetAlpha) < 0.01)
                this.hoverAlpha = targetAlpha;
        }

        void UpdateVariables()
        {
            if (!this.records.Any())
                return;

            this.firstPlace = this.records.Last();
            this.lastPlace = this.records.First();

            //this.newest = (float)TimeSpan.FromTicks(this.firstPlace.Date.Ticks).TotalSeconds;
            this.newestDateSeconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds;
            this.oldestDateSeconds = TimeSpan.FromTicks(this.lastPlace.Date.Ticks).TotalSeconds;
            //this.oldestDateSeconds = this.maxDateSeconds;

            this.fastestTotalSeconds = (float)this.firstPlace.InGameTime.TotalSeconds;
            this.slowestTotalSeconds = (float)TimeSpan.FromHours(this.maxHours).TotalSeconds;

            this.fastestHours = (int)Math.Round(this.firstPlace.InGameTime.TotalHours, MidpointRounding.AwayFromZero);
            this.slowestHours = (int)Math.Round(this.lastPlace.InGameTime.TotalHours, MidpointRounding.AwayFromZero);
        }

        void UpdateAvatars()
        {
            this.avatars.Clear();
            int counter = 0;
            foreach (Run run in this.records)
            {
                double runDateSeconds = TimeSpan.FromTicks(run.Date.Ticks).TotalSeconds;
                float x = (float)((this.oldestDateSeconds - runDateSeconds) / (this.oldestDateSeconds - this.newestDateSeconds));
                x *= this.graph.Inner.Width;
                x += this.graph.Inner.Left;

                float y = (this.slowestTotalSeconds - (float)run.InGameTime.TotalSeconds) / (this.slowestTotalSeconds - this.fastestTotalSeconds);

                y *= this.graph.Inner.Height;
                y += this.graph.Inner.Top;

                y = Math.Max(y, this.graph.Inner.Top);

                int halfSize = 8;
                var avatarBounds = new Rectangle((int)x - halfSize, (int)y - halfSize, halfSize * 2, halfSize * 2);
                this.avatars.Add((run, avatarBounds));
                counter++;
            }

            this.runHoverBounds.Clear();
            for (int i = 0; i < this.avatars.Count; i++)
            {
                (Run run, Rectangle bounds) = this.avatars[i];
                if (i < this.avatars.Count - 1)
                {
                    Rectangle nextBounds = this.avatars[i + 1].Item2;

                    var hoverBounds = new Rectangle(
                        bounds.Center.X, 
                        this.graph.Top + 8, 
                        nextBounds.Center.X - bounds.Center.X, 
                        this.graph.Height - 16);

                    this.runHoverBounds[hoverBounds] = run;
                }
                else
                {
                    var hoverBounds = new Rectangle(
                        bounds.Center.X,
                        this.graph.Top + 8,
                        this.graph.Right - bounds.Center.X,
                        this.graph.Height - 16);

                    this.runHoverBounds[hoverBounds] = run;
                }
            }
        }

        private void DrawHorizontalLines(Canvas canvas, int minutesBetween, float fadeStart, float fadeRange)
        {
            float alpha = fadeStart is 0
                ? 0.2f
                : (float)(Math.Min(fadeStart - this.maxHours, fadeRange) / fadeRange) * 0.2f;

            if (alpha is 0)
                return;

            Color color = ColorHelper.Fade(Config.Main.TextColor, alpha);

            int topBound = this.graph.Inner.Top ;
            int bottomBound = this.graph.Bottom - 8;
            int fastestMinutes = this.fastestHours * 60;
            float rangeSeconds = this.slowestTotalSeconds - this.fastestTotalSeconds;
            int maxMinutes = (int)Math.Round(this.maxHours * 60, MidpointRounding.AwayFromZero);

            int total = 0;
            for (double minutes = fastestMinutes; minutes < maxMinutes; minutes += minutesBetween)
            {
                double seconds = minutes * 60;
                float y = (float)(this.slowestTotalSeconds - seconds) / rangeSeconds;
                y *= this.graph.Inner.Height;
                y += this.graph.Inner.Top;

                if (y >= bottomBound || y <= topBound)
                    continue;

                const float FadeoutRange = 16;
                float distanceToTop = Math.Abs(y - topBound);
                float distanceToBottom = Math.Abs(y - bottomBound);
                float fadeoutMultiplier = 1.0f;
                if (distanceToTop <= FadeoutRange)
                    fadeoutMultiplier = distanceToTop / FadeoutRange;
                else if (distanceToBottom <= FadeoutRange)
                    fadeoutMultiplier = distanceToBottom / FadeoutRange;

                var line = new Rectangle(this.graph.Left + 8, (int)y, this.graph.Width - this.graph.Padding.Right - 9, 1);
                canvas.DrawRectangle(line, color * fadeoutMultiplier, null, 0, Layer.Main);

                var timespan = TimeSpan.FromMinutes(minutes);

                string labelText = $"{(int)timespan.TotalHours}:{timespan:mm':'ss}";
                Vector2 labelSize = this.font.MeasureString(labelText);
                var labelLocation = new Vector2(this.graph.Left - labelSize.X - 6, y - (labelSize.Y / 2) - 1);
                canvas.DrawString(this.font, labelText, labelLocation, color * fadeoutMultiplier, Layer.Main);
                total++;
            }
        }

        private void DrawVerticalLines(Canvas canvas)
        {
            if (this.records.Count == 0)
                return;

            float alpha = 0.2f;

            if (alpha is 0 || this.lastPlace is null)
                return;

            Color color = ColorHelper.Fade(Config.Main.TextColor, alpha);

            DateTime start = this.lastPlace.Date;
            DateTime end = DateTime.UtcNow;
            double tickRange = end.Ticks - start.Ticks;

            DateTime date = start;
            bool todayShown = false;
            while (date <= end || !todayShown)
            {
                float x = (float)((date - start).Ticks / tickRange);
                x *= this.graph.Inner.Width;
                x += this.graph.Inner.Left;

                x = Math.Min(x, this.graph.Right);
                x = Math.Max(x, this.graph.Left);

                var line = new Rectangle((int)x, this.graph.Top + 8, 1, this.graph.Height - 16);
                canvas.DrawRectangle(line, color, null, 0, Layer.Main);

                if (date == DateTime.Today)
                    todayShown = true;

                string labelText = date == DateTime.Today ? "Today" : date.ToShortDateString();
                Vector2 labelSize = this.font.MeasureString(labelText);
                var labelLocation = new Vector2(x - (labelSize.X / 2), this.graph.Bottom + 4);
                canvas.DrawString(this.font, labelText, labelLocation, color, Layer.Main);

                date = new DateTime(date.Year + 1, 1, 1);
                if (!todayShown && date > DateTime.Today)
                    date = DateTime.Today;
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            /*
            int padding = 16;
            var glowBounds = new Rectangle(
                this.Left - padding, 
                this.Top - padding,
                this.Width + (padding * 2),
                this.Height + (padding * 2));
            canvas.Draw("popup_block_back", glowBounds, Config.Main.TextColor);
            */

            base.DrawThis(canvas);

            canvas.DrawRectangle(this.graph.Bounds, Config.Main.BackColor, Config.Main.BorderColor, 8, Layer.Main);

            this.DrawHorizontalLines(canvas, 120, 0, 0);
            this.DrawHorizontalLines(canvas, 60, 20, 4);
            this.DrawHorizontalLines(canvas, 30, 10, 4);
            this.DrawHorizontalLines(canvas, 15, 5, 1);
            this.DrawHorizontalLines(canvas, 5, 4, 0.5f);
            this.DrawHorizontalLines(canvas, 1, 2.8f, 0.3f);

            this.DrawVerticalLines(canvas);

            this.DrawHover(canvas);

            this.DrawAvatars(canvas);
        }

        private void DrawHover(Canvas canvas)
        {
            if (this.hoveredBounds == Rectangle.Empty)
                return;

            Color color = ColorHelper.Fade(Config.Main.TextColor, this.hoverAlpha);

            if (this.hoveredBounds.Right <= this.graph.Inner.Right)
            {
                canvas.DrawRectangle(this.hoveredBounds, color, null, 0);
                canvas.DrawRectangle(new Rectangle(this.hoveredBounds.Left, this.hoveredBounds.Top, 1, this.hoveredBounds.Height), color, null, 0);
                canvas.DrawRectangle(new Rectangle(this.hoveredBounds.Right - 1, this.hoveredBounds.Top, 1, this.hoveredBounds.Height), color, null, 0);
            }
            else
            {
                int extra = this.hoveredBounds.Right - this.graph.Inner.Right;
                var clampedBounds = new Rectangle(
                    this.hoveredBounds.Left,
                    this.hoveredBounds.Top,
                    this.hoveredBounds.Width - extra,
                    this.hoveredBounds.Height);
                canvas.DrawRectangle(clampedBounds, color, null, 0);
                canvas.DrawRectangle(new Rectangle(clampedBounds.Left, clampedBounds.Top, 1, clampedBounds.Height), color, null, 0);
                canvas.DrawRectangle(new Rectangle(clampedBounds.Right - 1, clampedBounds.Top, 1, clampedBounds.Height), color, null, 0);
            }
        }

        void DrawAvatars(Canvas canvas)
        {
            for (int i = 0; i < this.avatars.Count; i++)
            {
                int halfThickness = 1;
                (Run run, Rectangle bounds) = this.avatars[i];
                if (i < this.avatars.Count - 1)
                {
                    Rectangle nextBounds = this.avatars[i + 1].Item2;
                    var lineH = new Rectangle(
                        bounds.Center.X,
                        bounds.Center.Y - halfThickness,
                        nextBounds.Center.X - bounds.Center.X,
                        halfThickness * 2);

                    var lineV = new Rectangle(
                        nextBounds.Center.X - halfThickness,
                        bounds.Center.Y - halfThickness,
                        halfThickness * 2,
                        nextBounds.Center.Y - bounds.Center.Y);

                    canvas.DrawRectangle(lineH, Config.Main.TextColor, null, 0);
                    canvas.DrawRectangle(lineV, Config.Main.TextColor, null, 0);
                }
                else
                {
                    var lineH = new Rectangle(
                        bounds.Center.X,
                        bounds.Center.Y - halfThickness,
                        this.graph.Inner.Right - bounds.Center.X - 1 ,
                        halfThickness * 2);
                    canvas.DrawRectangle(lineH, Config.Main.TextColor, null, 0);
                }

                canvas.Draw("avatar-" + Leaderboard.GetRealName(run.Runner).ToLower(), bounds, Color.White);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);

            this.runnerMode = Attribute(node, "runner_mode", this.runnerMode);
        }
    }
}

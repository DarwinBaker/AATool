using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net.Requests;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIRecordGraph : UIPanel
    {
        public string Category { get; private set; }
        public string Version { get; private set; }

        private bool upToDate;

        public bool LiveHistoryAvailable => SpreadsheetRequest.DownloadedPages.Contains(
            (Paths.Web.AASheet, Paths.Web.PrimaryAAHistory));

        readonly List<Run> records = new();
        
        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);

            //attempt to populate with cached or already downloaded data
            if (Leaderboard.History is not null)
                this.Populate(Leaderboard.History);

            if (!this.LiveHistoryAvailable)
            {
                new SpreadsheetRequest("history", Paths.Web.AASheet, Paths.Web.PrimaryAAHistory).EnqueueOnce();
                //new SpreadsheetRequest(Paths.Web.NicknameSheet).EnqueueOnce();
            }
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.upToDate && this.LiveHistoryAvailable)
            {
                this.Populate(Leaderboard.History);
                this.upToDate = true;
            }
        }

        private void Clear()
        {
            this.records.Clear();
        }

        
        private void Populate(LeaderboardSheet history)
        {
            //WIP
            string test = string.Empty;
            TimeSpan wr = TimeSpan.MaxValue;
            int row = 1;
            while (row <= history.Rows.Length)
            {
                if (Run.TryParse(history, row, "1.16", out Run run))
                {
                    wr = run.InGameTime;
                    this.records.Add(run);
                    test += $"{run.Runner} - {wr}\n";
                }
                row++;
            }
        }

        private void RefreshLayout()
        { 
            
        }

        public override void DrawThis(Canvas canvas)
        {
            base.DrawThis(canvas);
            if (this.records.Count <= 1)
                return;

            Run firstPlace = this.records.Last();
            Run lastPlace = this.records.First();

            double newest = TimeSpan.FromTicks(firstPlace.Date.Ticks).TotalSeconds;
            double oldest = TimeSpan.FromTicks(lastPlace.Date.Ticks).TotalSeconds;
            double dateRange = oldest - newest;

            double slowest = lastPlace.InGameTime.TotalSeconds;
            double fastest = firstPlace.InGameTime.TotalSeconds;
            double timeRange = slowest - fastest;

            int slowestHours = (int)Math.Round(lastPlace.InGameTime.TotalHours, MidpointRounding.AwayFromZero);
            int fastestHours = (int)Math.Round(firstPlace.InGameTime.TotalHours, MidpointRounding.AwayFromZero);

            for (int i = fastestHours; i < slowestHours; i++)
            {
                double y = (slowest - (i * 60 * 60)) / (slowest - fastest);
                y *= this.Inner.Height;
                y += this.Inner.Top;
                var line = new Rectangle(this.Left, (int)y, this.Bounds.Width, 1);
                canvas.DrawRectangle(line, Color.White * 0.4f, null, 0, Layer.Fore);
            }

            foreach (Run run in this.records)
            {
                double runDateSeconds = TimeSpan.FromTicks(run.Date.Ticks).TotalSeconds;
                double x = (oldest - runDateSeconds) / (oldest - newest);
                x *= this.Inner.Width;
                x += this.Inner.Left;

                double y = (slowest - run.InGameTime.TotalSeconds) / (slowest - fastest);
                y *= this.Inner.Height;
                y += this.Inner.Top;

                int halfSize = 8;
                var avatar = new Rectangle((int)x - halfSize, (int)y - halfSize, halfSize * 2, halfSize * 2);

                canvas.Draw("avatar-" + run.Runner.ToLower(), avatar, Color.White, Layer.Fore);
            }
        }
    }
}

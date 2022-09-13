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
        private readonly Dictionary<string, UIPersonalBest> records;

        public string Category { get; private set; }
        public string Version { get; private set; }

        private bool upToDate;

        public bool LiveHistoryAvailable => SpreadsheetRequest.DownloadedPages.Contains((Paths.Web.AASheet, Paths.Web.PrimaryAAHistory));
        
        /*
        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);

            //attempt to populate with cached or already downloaded data
            if (Leaderboard.History is not null)
                this.Populate(Leaderboard.History);

            if (!this.LiveHistoryAvailable)
            {
                //new SpreadsheetRequest(Paths.Web.AASheet, Paths.Web.PrimaryAAHistory).EnqueueOnce();
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
            string records = string.Empty;
            bool valid = true;
            int row = 1;
            TimeSpan wr = TimeSpan.MaxValue;
            while (valid)
            {
                //valid = Run.TryParse(history, row, out Run run);
                if (valid && run.InGameTime < wr)
                {
                    wr = run.InGameTime;
                    records += $"{run.Runner} - {wr.ToString()}\n";
                }
                row++;
            }
        }
        */
    }
}

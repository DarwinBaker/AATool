using System.Linq;
using AATool.Data.Speedrunning;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIRecordHolderHalfHeartHardcore : UIRecordHolder
    {
        public UIRecordHolderHalfHeartHardcore() : base()
        {
        }

        protected override void ProfileButtonClick(UIControl sender)
        {
            if (UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
            {
                if (Leaderboard.HalfHeartHardcoreCompletions?.Runs?.FirstOrDefault() is Run wr)
                {
                    RunnerProfile.SetCurrentName(wr.Runner);
                    UIMainScreen.SetActiveTab(UIMainScreen.RunnerProfileTab);
                }
            }
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.Populate();
        }

        protected override void RequestRefresh()
        {
            if (!this.LiveBoardAvailable)
                new SpreadsheetRequest("history_aa_1.16", Paths.Web.AASheet, Paths.Web.PrimaryAAHistory).EnqueueOnce();
        }

        protected override void Populate()
        {
            this.Title.SetText("1.16 AA");
            this.Subtitle.SetText("Half-Heart Hardcore");

            if (Leaderboard.HalfHeartHardcoreCompletions?.Runs?.FirstOrDefault() is not Run wr)
                return;

            new AvatarRequest(wr.Runner).EnqueueOnce();
            this.Avatar.SetPlayer(wr.Runner);
            this.SetBadge();

            string mostRecordsList = string.Empty;
            for (int i = 0; i < Leaderboard.ListOfMostConcurrentRecords.Count; i++)
            {
                mostRecordsList += Leaderboard.ListOfMostConcurrentRecords[i].GameVersion;
                if (i < Leaderboard.ListOfMostConcurrentRecords.Count - 1)
                    mostRecordsList += ", ";
            }
            this.Runner.SetText(wr.Runner);
            this.Details.SetText($"{wr.InGameTime:h':'mm':'ss} IGT");
        }

        protected override void SetBadge()
        {
            this.Avatar.SetBadge(new HalfHeartHardcoreBadge());
        }
    }
}

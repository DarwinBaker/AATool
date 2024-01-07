using System.Linq;
using AATool.Data.Speedrunning;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIRecordHolderMostConcurrent : UIRecordHolder
    {
        public UIRecordHolderMostConcurrent() : base()
        {
        }

        protected override void ProfileButtonClick(UIControl sender)
        {
            if (UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
            {
                if (!string.IsNullOrEmpty(Leaderboard.RunnerWithMostConcurrentRecords))
                {
                    RunnerProfile.SetCurrentName(Leaderboard.RunnerWithMostConcurrentRecords);
                    UIMainScreen.SetActiveTab(UIMainScreen.RunnerProfileTab);
                }
            }
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.Populate();
        }

        protected override void Populate()
        {
            this.Title.SetText("Most AA WRs");
            this.Subtitle.SetText("Concurrent Versions");

            new AvatarRequest(Leaderboard.RunnerWithMostConcurrentRecords).EnqueueOnce();
            this.Avatar.SetPlayer(Leaderboard.RunnerWithMostConcurrentRecords);
            this.SetBadge();

            string mostRecordsList = string.Empty;
            for (int i = 0; i < Leaderboard.ListOfMostConcurrentRecords.Count; i++)
            {
                mostRecordsList += Leaderboard.ListOfMostConcurrentRecords[i].GameVersion;
                if (i < Leaderboard.ListOfMostConcurrentRecords.Count - 1)
                    mostRecordsList += ", ";
            }
            this.Runner.SetText(Leaderboard.RunnerWithMostConcurrentRecords);
            this.Details.SetText(mostRecordsList);
        }

        protected override void SetBadge()
        {
            this.Avatar.SetBadge(new MostConcurrentRecordsBadge());
        }
    }
}

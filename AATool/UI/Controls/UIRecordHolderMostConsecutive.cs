using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Data.Speedrunning;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIRecordHolderMostConsecutive : UIRecordHolder
    {
        public UIRecordHolderMostConsecutive() : base()
        {
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.Populate();
        }

        protected override void ProfileButtonClick(UIControl sender)
        {
            if (UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
            {
                if (!string.IsNullOrEmpty(Leaderboard.RunnerWithMostConsecutiveRecords))
                {
                    RunnerProfile.SetCurrentName(Leaderboard.RunnerWithMostConsecutiveRecords);
                    UIMainScreen.SetActiveTab(UIMainScreen.RunnerProfileTab);
                }
            }
        }

        protected override void Populate()
        {
            this.Title.SetText("Most AA WRs");
            this.Subtitle.SetText("Consecutive, 1.16");

            if (Leaderboard.MostConsecutiveRecordsCount < 1 || string.IsNullOrEmpty(Leaderboard.RunnerWithMostConsecutiveRecords))
                return;

            new AvatarRequest(Leaderboard.RunnerWithMostConsecutiveRecords).EnqueueOnce();
            this.Avatar.SetPlayer(Leaderboard.RunnerWithMostConsecutiveRecords);
            this.SetBadge();
            
            this.Runner.SetText(Leaderboard.RunnerWithMostConsecutiveRecords);
            this.Details.SetText($"{Leaderboard.MostConsecutiveRecordsCount} records back-to-back");
        }

        protected override void SetBadge()
        {
            this.Avatar.SetBadge(new MostConsecutiveRecordsBadge());
        }
    }
}

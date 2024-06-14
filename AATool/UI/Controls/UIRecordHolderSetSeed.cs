using System.Linq;
using AATool.Data.Speedrunning;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIRecordHolderSetSeed : UIRecordHolder
    {
        public override bool LiveBoardAvailable => base.LiveBoardAvailable;

        public UIRecordHolderSetSeed() : base()
        {
        }

        protected override void ProfileButtonClick(UIControl sender)
        {
            if (UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
            {
                if (!string.IsNullOrEmpty(Leaderboard.AASsgRunner))
                {
                    RunnerProfile.SetCurrentName(Leaderboard.AASsgRunner);
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
            new AASsgRequest().EnqueueOnce();
        }

        protected override void Populate()
        {
            this.Title.SetText("1.16 AA");
            this.Subtitle.SetText("SSG");

            if (string.IsNullOrEmpty(Leaderboard.AASsgRunner))
                return;

            new AvatarRequest(Leaderboard.AASsgRunner).EnqueueOnce();
            this.Avatar.SetPlayer(Leaderboard.AASsgRunner);
            this.SetBadge();
            
            this.Runner.SetText(Leaderboard.AASsgRunner);
            this.Details.SetText($"{Leaderboard.AASsgInGameTime:h':'mm':'ss} IGT    {Leaderboard.AASsgRealTime:h':'m':'ss} RTA");
        }

        protected override void SetBadge()
        {
            this.Avatar.SetBadge(new RankBadge(1, "AA SSG", "1.6"));
        }
    }
}

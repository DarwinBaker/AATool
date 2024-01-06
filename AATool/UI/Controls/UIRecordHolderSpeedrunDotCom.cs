using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Data.Speedrunning;
using AATool.Net.Requests;
using AATool.UI.Screens;

namespace AATool.UI.Controls
{
    internal class UIRecordHolderSpeedrunDotCom : UIRecordHolder
    {
        public override bool LiveBoardAvailable 
            => SrcLeaderboardRequest.DownloadedLeaderboards.Contains((this.Category, this.Version));
        
        public override void InitializeThis(UIScreen screen)
        {
            if (!Leaderboard.TryGet(this.Category, this.Version, out _))
                Leaderboard.TryLoadCachedSrc(this.Category, this.Version, out _);

            base.InitializeThis(screen);
        }

        protected override void RequestRefresh()
        {
            if (!this.LiveBoardAvailable)
                new SrcLeaderboardRequest(this.Category, this.Version).EnqueueOnce();
        }
    }
}

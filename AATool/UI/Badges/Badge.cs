using AATool.Data.Players;
using AATool.Net;
using AATool.UI.Controls;

namespace AATool.UI.Badges
{
    public static class Badge
    {
        //completed the first ever half-heart hardcore all advancements speedrun
        private static readonly Uuid Elysaku = new ("b2fcb273-9886-4a9b-bd7f-e005816fb7b7");
        private const string ElysakuName = "elysaku";

        //completed 1000 any% RSG speedruns in a row without resetting
        private static readonly Uuid Couriway = new ("994f9376-3f80-48bc-9e72-ee92f861911d");
        private const string CouriwayName = "couriway";

        public static bool TryGet(Uuid player, string name, int scale, out UIControl badge)
        {
            badge = null;
            player = new Uuid(player.String);
            name = name?.ToLower();
            bool hasWorldRecord = Leaderboard.TryGetRank(name, out int rank) && rank is 1;
            if (hasWorldRecord)
            {
                //award wr badge if player holds the top time on the current leaderboard
                badge = new RecordHolderBadge(2, rank);
            }
            else if (player.String == Elysaku || name is ElysakuName)
            {
                //award elden's hhhaa badge
                badge = new ImmortalBadge(scale);
            }
            else if (player.String == Couriway || name is CouriwayName)
            {
                //reward couri's 1000 seeds badge
                badge = new UnstoppableBadge(scale);
            }
            else
            {
                //award pb badge if player holds a top 10 time on the current leaderboard
                if (rank is > 0 and <= 10)
                    badge = new RecordHolderBadge(2, rank);
            }
            return badge is not null;
        }

        public static bool TryGet(string player, int scale, out UIControl badge)
        {
            badge = null;
            if (string.IsNullOrEmpty(player))
                return false;

            string leaderboardName = Leaderboard.GetNickName(player).ToLower();
            if (leaderboardName is ElysakuName)
            {
                //award elden's hhhaa badge
                badge = new ImmortalBadge(scale);
            }
            else if (leaderboardName is CouriwayName)
            {
                //reward couri's 1000 seeds badge
                badge = new UnstoppableBadge(scale);
            }
            else
            {
                //award pb/wr badge if player holds a top 10 time on the current leaderboard
                if (Leaderboard.TryGetRank(leaderboardName, out int rank) && rank <= 10)
                    badge = new RecordHolderBadge(2, rank);
            }
            return badge is not null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AATool.Data.Categories;
using AATool.Net;
using AATool.UI.Controls;

namespace AATool.Data.Players
{
    public sealed class Leaderboard
    {
        private static Dictionary<string, Leaderboard> AllBoards = new ();
        private static HashSet<string> LiveBoards = new ();
        private static Dictionary<string, string> NickNames = new ();
        private static Dictionary<string, string> RealNames = new ();

        public static readonly TimeZoneInfo TimeZone = TimeZoneInfo
            .FindSystemTimeZoneById("Eastern Standard Time");

        public Dictionary<string, int> Ranks = new ();
        public List<PersonalBest> Runs = new ();

        private LeaderboardSheet sheet;

        public static bool NickNamesLoaded { get; private set; }

        public static string Current => $"{Tracker.Category.Name} {Tracker.Category.CurrentMajorVersion}";
        public static string CurrentBoardHeader => $"{Tracker.Category.CurrentMajorVersion} rsg";
        public static bool IsLiveAvailable => LiveBoards.Contains(Current);

        public static string GetRealName(string runner) =>
            !string.IsNullOrEmpty(runner) && RealNames.TryGetValue(runner.ToLower(), out string real) ? real : runner;

        public static string GetNickName(string runner) => 
            !string.IsNullOrEmpty(runner) && NickNames.TryGetValue(runner.ToLower(), out string nick) ? nick : runner;

        public static string GetKey(string page) =>
            page is Paths.Web.PrimaryVersionBoard ? "leaderboard_primary" : "leaderboard_others";

        public Leaderboard(LeaderboardSheet sheet)
        {
            this.sheet = sheet;
            lock (this.Ranks)
            {
                for (int i = 2; i < this.sheet.Rows.Length; i++)
                {
                    if (PersonalBest.TryParse(this.sheet, i, out PersonalBest pb))
                    {
                        this.Runs.Add(pb);
                        this.Ranks[pb.Runner.ToLower()] = i - 1;

                        if (this.Runs.Count <= UILeaderboard.Rows)
                            Player.FetchIdentity(pb.Runner);
                    }
                }
            }
        }

        public static bool TryGet(string boardName, out Leaderboard leaderboard)
        {
            if (AllBoards.TryGetValue(boardName, out leaderboard))
                return true;
            return TryLoadCached(boardName, out leaderboard);
        }

        public static bool TryGetRank(string runner, out int rank)
        {
            if (!string.IsNullOrEmpty(runner))
            {
                if (AllBoards.TryGetValue(Current, out Leaderboard board))
                {
                    lock (board.Ranks)
                    {
                        //try player's real ingame name
                        if (board.Ranks.TryGetValue(GetRealName(runner).ToLower(), out rank))
                            return true;
                        //try player's preferred leaderboard nickname
                        if (board.Ranks.TryGetValue(GetNickName(runner).ToLower(), out rank))
                            return true;
                    }

                }
            }
            rank = 0;
            return false;
        }

        public static bool SyncRecords(string page, string csv)
        {
            string key = GetKey(page);
            if (page is Paths.Web.PrimaryVersionBoard)
            {
                if (LeaderboardSheet.TryParse(csv, key, null, out LeaderboardSheet sheet))
                {
                    AllBoards["All Advancements 1.16"] = new Leaderboard(sheet);
                    LiveBoards.Add("All Advancements 1.16");
                    sheet.SaveToCache();
                    return true;
                }
            }
            else
            {
                if (LeaderboardSheet.TryParse(csv, key, CurrentBoardHeader, out LeaderboardSheet sheet))
                {
                    AllBoards[Current] = new Leaderboard(sheet);
                    LiveBoards.Add(Current);
                    sheet.SaveToCache();
                    return true;
                }
            }
            return false;
        }

        public static bool SyncNicknames(string csv)
        {
            if (NicknameSheet.TryParse(csv, out NicknameSheet sheet))
            {
                sheet.GetMappings(out RealNames, out NickNames);
                NickNamesLoaded = true;
                sheet.SaveToCache();
            }
            return NickNamesLoaded;
        }

        public bool TryGetRank(string runner, out string place)
        {
            place = string.Empty;
            if (!this.Ranks.TryGetValue(runner.ToLower(), out int ranking) || ranking < 1)
                return false;

            if (ranking % 100 is 11 or 12 or 13)
            {
                place = ranking + "th";
            }
            else
            {
                place = (ranking % 10) switch {
                    1 => ranking + "st",
                    2 => ranking + "nd",
                    3 => ranking + "rd",
                    _ => ranking + "th",
                };
            }
            return true;
        }

        private static bool TryLoadCached(string boardName, out Leaderboard leaderboard)
        {
            leaderboard = null;
            string page = boardName == "All Advancements 1.16"
                ? Paths.Web.PrimaryVersionBoard
                : Paths.Web.OtherVersionsBoard;
            string header = boardName == "All Advancements 1.16"
                ? null : CurrentBoardHeader;

            string key = GetKey(page);
            string leaderboardFile = Paths.System.LeaderboardFile(key);
            if (File.Exists(leaderboardFile))
            {
                try
                {
                    string csv = File.ReadAllText(leaderboardFile);
                    if (LeaderboardSheet.TryParse(csv, key, header, out LeaderboardSheet sheet))
                        AllBoards[boardName] = leaderboard = new Leaderboard(sheet);
                }
                catch
                {
                    //couldn't read cached leaderboard, move on
                }
            }

            string namesFile = Paths.System.LeaderboardFile("leaderboard_names");
            if (File.Exists(namesFile))
            {
                try
                {
                    string csv = File.ReadAllText(namesFile);
                    if (NicknameSheet.TryParse(csv, out NicknameSheet sheet))
                    sheet.GetMappings(out RealNames, out NickNames);
                }
                catch
                {
                    //couldn't read cached nickname mappings, move on
                }
            }
            return leaderboard is not null;
        }
    }
}

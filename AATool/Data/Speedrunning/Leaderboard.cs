using System;
using System.Collections.Generic;
using System.IO;

namespace AATool.Data.Speedrunning
{
    public sealed class Leaderboard
    {
        private static readonly Dictionary<(string category, string version), Leaderboard> AllBoards = new ();
        private static readonly HashSet<(string category, string version)> LiveBoards = new ();
        private static readonly HashSet<string> RequestedIdentities = new ();
        private static Dictionary<string, string> NickNames = new ();
        private static Dictionary<string, string> RealNames = new ();

        public static LeaderboardSheet History { get; private set; }

        public static readonly TimeZoneInfo TimeZone = TimeZoneInfo
            .FindSystemTimeZoneById("Eastern Standard Time");

        public string Category { get; private set; }
        public string Version { get; private set; }

        public Dictionary<string, int> Ranks = new ();
        public List<Run> Runs = new ();

        private readonly LeaderboardSheet sheet;

        public int TotalRows => this.sheet?.Rows.Length ?? 0;

        public static bool NickNamesLoaded { get; private set; }
        public static (string category, string version) Current => (Tracker.Category.Name, Tracker.Category.CurrentMajorVersion);
        public static bool IsLiveAvailable(string cateogy, string version) => LiveBoards.Contains((cateogy, version));
        public static bool IdentityAlreadyRequested(string name) => RequestedIdentities.Contains(name);

        public static string GuidanceHeader(string cateogy, string version)
        {
            //1.16 aa has its own separate page
            return (cateogy is "All Advancements" && version is "1.16") || cateogy is "All Blocks"
                ? null
                : $"{version} rsg";
        }

        public static string GetRealName(string runner) =>
            !string.IsNullOrEmpty(runner) && RealNames.TryGetValue(runner.ToLower(), out string real) ? real : runner;

        public static string GetNickName(string runner) =>
            !string.IsNullOrEmpty(runner) && NickNames.TryGetValue(runner.ToLower(), out string nick) ? nick : runner;

        public static string GetKey(string category, string version)
        {
            if (category is "All Blocks")
                return $"leaderboard_all_blocks_{version}";
            return version is "1.16" ? "leaderboard_aa_primary" : "leaderboard_aa_others";
        }

        public Leaderboard(LeaderboardSheet sheet, string category, string version)
        {
            this.Category = category;
            this.Version = version;
            this.sheet = sheet;
            lock (this.Ranks)
            {
                for (int i = 2; i < this.sheet.Rows.Length; i++)
                {
                    if (Run.TryParse(this.sheet, i, out Run pb))
                    {
                        this.Runs.Add(pb);
                        this.Ranks[pb.Runner.ToLower()] = i - 1;
                    }
                }
            }
        }

        public static bool TryGet(string category, string version, out Leaderboard leaderboard)
        {
            return AllBoards.TryGetValue((category, version), out leaderboard)
                ? leaderboard is not null
                : TryLoadCached(category, version, out leaderboard);
        }

        public static bool TryGetRank(string runner, string category, string version, out int rank)
        {
            if (!string.IsNullOrEmpty(runner))
            {
                if (AllBoards.TryGetValue((category, version), out Leaderboard board) && board is not null)
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

        public static bool SyncRecords(string sheetId, string pageId, string csv)
        {
            LeaderboardSheet sheet = null;
            string category = sheetId switch {
                Paths.Web.AASheet => "All Advancements",
                Paths.Web.ABSheet => "All Blocks",
                _ => string.Empty
            };
            if (string.IsNullOrEmpty(category))
                return false;

            List<string> versions;
            if (sheetId is Paths.Web.AASheet && pageId is Paths.Web.AAPage16)
                versions = new () { "1.16" };
            else if (sheetId is Paths.Web.ABSheet && pageId is Paths.Web.ABPage16)
                versions = new() { "1.16" };
            else if (pageId is Paths.Web.ABPage18)
                versions = new() { "1.18" };
            else if (pageId is Paths.Web.ABPage19)
                versions = new() { "1.19" };
            else
                versions = new () { "1.19", "1.18", "1.17", "1.15", "1.14", "1.13", "1.12", "1.11", "1.6" };

            //parse all the leaderboards
            foreach (string version in versions)
            {
                if (LeaderboardSheet.TryParse(csv, GetKey(category, version), GuidanceHeader(category, version), out sheet))
                {
                    AllBoards[(category, version)] = new Leaderboard(sheet, category, version);
                    LiveBoards.Add((category, version));
                }
            }
            sheet?.SaveToCache();
            return sheet is not null;
        }

        public static bool SyncHistory(string csv)
        {
            if (LeaderboardSheet.TryParse(csv, "history_1.16", null, out LeaderboardSheet sheet))
                History = sheet;
            return sheet is not null;
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

        private static bool TryLoadCached(string category, string version, out Leaderboard leaderboard)
        {
            leaderboard = null;
            string key = GetKey(category, version);
            string leaderboardFile = Paths.System.LeaderboardFile(key);
            if (File.Exists(leaderboardFile))
            {
                try
                {
                    string csv = File.ReadAllText(leaderboardFile);
                    string header = GuidanceHeader(category, version);
                    if (LeaderboardSheet.TryParse(csv, key, header, out LeaderboardSheet sheet))
                        AllBoards[(category, version)] = leaderboard = new Leaderboard(sheet, category, version);
                    else
                        AllBoards[(category, version)] = null;
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

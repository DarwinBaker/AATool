using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AATool.Net;
using AATool.Net.Requests;
using AATool.UI.Badges;

namespace AATool.Data.Speedrunning
{
    public sealed class Leaderboard
    {
        private static readonly Dictionary<(string category, string version), Leaderboard> AllBoards = new ();
        private static readonly HashSet<(string category, string version)> LiveBoards = new ();
        private static readonly HashSet<string> RequestedIdentities = new ();

        private static Dictionary<string, string> NickNames = new ();
        private static Dictionary<string, string> RealNames = new ();
        private static Dictionary<string, Uuid> Identities = new ();

        private static HashSet<string> AllRunnerNames = new ();
        private static HashSet<Uuid> AllRunners = new ();

        public static List<Run> ListOfMostRecords { get; private set; } = new();
        public static string RunnerWithMostWorldRecords { get; private set; } = string.Empty;
        public static LeaderboardSheet History { get; private set; }

        public static string RsgRunner { get; private set; }
        public static TimeSpan RsgInGameTime { get; private set; }
        public static TimeSpan RsgRealTime { get; private set; }

        public static string SsgRunner { get; private set; }
        public static TimeSpan SsgInGameTime { get; private set; }
        public static TimeSpan SsgRealTime { get; private set; }


        public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public static readonly string[] AAVersions = { "1.19", "1.18", "1.17", "1.16", "1.15", "1.14", "1.13", "1.12", "1.11", "1.6" };

        public string Category { get; private set; }
        public string Version { get; private set; }

        public Dictionary<string, int> Ranks = new ();
        public List<Run> Runs = new ();

        private readonly LeaderboardSheet sheet;

        public int TotalRows => this.sheet?.Rows.Length ?? 0;

        public static bool NickNamesLoaded { get; private set; }
        public static (string category, string version) Current => (Tracker.Category.Name, Tracker.Category.CurrentMajorVersion);
        public static bool IsLiveAvailable(string category, string version) => LiveBoards.Contains((category, version));
        public static bool IdentityAlreadyRequested(string name) => RequestedIdentities.Contains(name);
        public static bool IsRunner(Uuid player, string name = null) => AllRunners.Contains(player) || AllRunnerNames.Contains(name);

        public static void Initialize()
        {
            foreach (string version in AAVersions)
                TryLoadCached("All Advancements", version, out _);
            TryLoadCachedAnyPercent(true, "1.16", out _);
            TryLoadCachedAnyPercent(false, "1.16", out _);
            UpdateMostWorldRecords();
        }

        public static void Refresh(string category = null, string version = null)
        {
            NetRequest.ClearHistory();
            Player.IdentitiesAlreadyRequested.Clear();
            Player.NamesAlreadyRequested.Clear();
            SpreadsheetRequest.DownloadedPages.Clear();
            Badge.HoverTimer.Reset();
            NickNamesLoaded = false;
            if (category is null)
            {
                LiveBoards.Clear();
            }
            else
            {
                LiveBoards.Remove((category, version));
            }
        }

        public static string GuidanceHeader(string cateogy, string version)
        {
            //1.16 aa has its own separate page
            return (cateogy is "All Advancements" && version is "1.16") || cateogy is "All Blocks"
                ? null
                : $"{version} rsg";
        }

        public static bool TryGetIdentity(string runner, out Uuid uuid)
        {
            uuid = Uuid.Empty;
            if (!string.IsNullOrEmpty(runner))
                Identities.TryGetValue(runner.ToLower(), out uuid);
            return uuid != Uuid.Empty;
        }

        public static string GetRealName(string runner, string fallback = null) =>
            !string.IsNullOrEmpty(runner) && RealNames.TryGetValue(runner.ToLower(), out string real) ? real : fallback ?? runner;

        public static string GetNickName(string runner, string fallback = null) =>
            !string.IsNullOrEmpty(runner) && NickNames.TryGetValue(runner.ToLower(), out string nick) ? nick : fallback ?? runner;

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
                    if (Run.TryParse(this.sheet, i, this.Version, out Run pb))
                    {
                        int rank = i - 1;
                        this.Runs.Add(pb);
                        this.Ranks[pb.Runner.ToLower()] = rank;

                        string realName = GetRealName(pb.Runner);
                        _= AllRunnerNames.Add(realName);
                        _= AllRunnerNames.Add(pb.Runner);
                        if (Player.TryGetUuid(realName, out Uuid id))
                            _= AllRunners.Add(id);
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
            rank = 0;
            if (string.IsNullOrEmpty(runner))
                return false;

            if (!AllBoards.TryGetValue((category, version), out Leaderboard board) || board is null)
                return false;

            lock (board.Ranks)
            {
                board.Ranks.TryGetValue(GetRealName(runner).ToLower(), out int ignRank);
                board.Ranks.TryGetValue(GetNickName(runner).ToLower(), out int nickRank);

                if (ignRank > 0 && nickRank > 0)
                    rank = Math.Min(ignRank, nickRank);
                else if (ignRank + nickRank > 0)
                    rank = Math.Max(ignRank, nickRank);
            }
            return rank is not int.MaxValue;
        }

        public static bool TryGetRank(Uuid runner, string category, string version, out int rank)
        {
            rank = 0;
            if (runner == Uuid.Empty)
                return false;

            if (!AllBoards.TryGetValue((category, version), out Leaderboard board) || board is null)
                return false;

            lock (board.Ranks)
            {
                board.Ranks.TryGetValue(GetRealName(runner.String).ToLower(), out int ignRank);
                board.Ranks.TryGetValue(GetNickName(runner.String).ToLower(), out int nickRank);

                if (ignRank > 0 && nickRank > 0)
                    rank = Math.Min(ignRank, nickRank);
                else if (ignRank + nickRank > 0)
                    rank = Math.Max(ignRank, nickRank);
            }
            return rank is not int.MaxValue;
        }

        public static bool TryGetWorldRecord(string category, string version, out Run wr)
        {
            wr = null;
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(version))
                return false;
            if (!AllBoards.TryGetValue((category, version), out Leaderboard board) || board is null)
                return false;
            if (board.Runs.Any())
                wr = board.Runs[0];
            return wr is not null;
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
            UpdateMostWorldRecords();
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
                sheet.GetMappings(out RealNames, out NickNames, out Identities);
                NickNamesLoaded = true;
                sheet.SaveToCache();
            }
            return NickNamesLoaded;
        }

        public static bool SyncAnyPercentRecord(string jsonString, bool rsg)
        {
            try
            {
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
                string runner = data["data"]["players"]["data"][0]["names"]["international"].Value;
                new AvatarRequest(runner).EnqueueOnce();

                string inGameString = data["data"]["runs"][0]["run"]["times"]["ingame_t"].Value.ToString();
                if (!double.TryParse(inGameString, out double inGameSeconds))
                    return false;
                string realTimeString = data["data"]["runs"][0]["run"]["times"]["realtime_t"].Value.ToString();
                if (!double.TryParse(realTimeString, out double realTimeSeconds))
                    return false;

                var igt = TimeSpan.FromSeconds(inGameSeconds);
                var rta = TimeSpan.FromSeconds(realTimeSeconds);

                if (rsg)
                {
                    RsgRunner = runner;
                    RsgInGameTime = igt;
                    RsgRealTime = rta;
                }
                else
                {
                    SsgRunner = runner;
                    SsgInGameTime = igt;
                    SsgRealTime = rta;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetPlace(int rank)
        {
            if (rank % 100 is 11 or 12 or 13)
            {
                return rank + "th";
            }
            else
            {
                return (rank % 10) switch {
                    1 => rank + "st",
                    2 => rank + "nd",
                    3 => rank + "rd",
                    _ => rank + "th",
                };
            }
        }

        public static void SaveAnyPercentRecordToCache(string jsonString, bool rsg)
        {
            try
            {
                //cache leaderboard so it loads instantly next launch
                //overwrite to keep leaderboard up to date
                Directory.CreateDirectory(Paths.System.LeaderboardsFolder);
                string path = Paths.System.AnyPercentRecordFile(rsg, "1.16");
                File.WriteAllText(path, jsonString);
            }
            catch
            {
                //couldn't save file. ignore and move on
            }
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
                        sheet.GetMappings(out RealNames, out NickNames, out Identities);
                }
                catch
                {
                    //couldn't read cached nickname mappings, move on
                }
            }
            return leaderboard is not null;
        }

        private static bool TryLoadCachedAnyPercent(bool rsg, string version, out string jsonString)
        {
            jsonString = string.Empty;
            string leaderboardFile = Paths.System.AnyPercentRecordFile(rsg, version);
            if (File.Exists(leaderboardFile))
            {
                try
                {
                    jsonString = File.ReadAllText(leaderboardFile);
                    SyncAnyPercentRecord(jsonString, rsg);
                }
                catch
                {
                    //couldn't read cached leaderboard, move on 
                }
            }
            return jsonString is not null;
        }

        private static void UpdateMostWorldRecords()
        {
            Dictionary<string, List<Run>> recordHolders = new ();
            foreach (string version in AAVersions)
            {
                if (!TryGetWorldRecord("All Advancements", version, out Run wr))
                    return;

                if (!recordHolders.TryGetValue(wr.Runner, out List<Run> records))
                    recordHolders[wr.Runner] = records = new List<Run>();
                records.Add(wr);
            }
            Version mostLatestVersion = default;
            int mostRecordsCount = 0;
            foreach (KeyValuePair<string, List<Run>> recordHolder in recordHolders)
            {
                bool foundNew = recordHolder.Value.Count > mostRecordsCount
                    || (recordHolder.Value.Count == mostRecordsCount && recordHolder.Value.LastOrDefault().GameVersion > mostLatestVersion);

                if (foundNew)
                {
                    mostRecordsCount = recordHolder.Value.Count;
                    RunnerWithMostWorldRecords = recordHolder.Value.LastOrDefault().Runner;
                    mostLatestVersion = recordHolder.Value.LastOrDefault().GameVersion;
                }
            }
            ListOfMostRecords.Clear();
            foreach (Run run in recordHolders[RunnerWithMostWorldRecords])
                ListOfMostRecords.Add(run);

            /*
            UIAvatar avatar = this.Root().First<UIAvatar>("most_records_avatar");
            avatar?.SetPlayer(mostRecordsName);
            //avatar?.RegisterOnLeaderboard(this.board);
            //avatar?.RefreshBadge();
            this.Root().First<UITextBlock>("most_records_runner")?.SetText(mostRecordsName);
            this.Root().First<UITextBlock>("most_records_list")?.SetText(mostRecordsList);
            */
        }
    }
}

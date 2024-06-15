using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AATool.Configuration;
using AATool.Net;
using AATool.Net.Requests;
using AATool.UI.Badges;
using Renci.SshNet;

namespace AATool.Data.Speedrunning
{
    public sealed class Leaderboard
    {
        public static readonly Dictionary<(string category, string version), Leaderboard> AllBoards = new ();
        private static readonly HashSet<(string category, string version)> LiveBoards = new ();
        private static readonly HashSet<string> RequestedIdentities = new ();

        private static Dictionary<string, string> NickNames = new ();
        private static Dictionary<string, string> RealNames = new ();
        private static Dictionary<string, Uuid> Identities = new ();

        private static HashSet<string> AllRunnerNames = new ();
        private static HashSet<Uuid> AllRunners = new ();

        public static Dictionary<(string runner, DateTime date), string> AALinks { get; private set; } = new();
        public static Leaderboard HalfHeartHardcoreCompletions { get; private set; }
        public static List<Run> HundredHardcoreCompletions { get; private set; } = new();
        public static List<Run> ListOfMostConcurrentRecords { get; private set; } = new();
        public static string RunnerWithMostConcurrentRecords { get; private set; } = string.Empty;
        public static int MostConsecutiveRecordsCount { get; private set; }
        public static string RunnerWithMostConsecutiveRecords { get; private set; } = string.Empty;
        public static LeaderboardSheet History { get; private set; }

        public static string AnyRsgRunner { get; private set; }
        public static TimeSpan AnyRsgInGameTime { get; private set; }
        public static TimeSpan AnyRsgRealTime { get; private set; }

        public static string AnySsgRunner { get; private set; }
        public static TimeSpan AnySsgInGameTime { get; private set; }
        public static TimeSpan AnySsgRealTime { get; private set; }

        public static string AASsgRunner { get; private set; }
        public static TimeSpan AASsgInGameTime { get; private set; }
        public static TimeSpan AASsgRealTime { get; private set; }


        public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public static readonly string[] AAVersions = { "1.20", "1.19", "1.18", "1.17", "1.16", "1.15", "1.14", "1.13", "1.12", "1.11", "1.6" };
        public static readonly string[] AnyPercentVersions = { "1.16+", "1.13-1.15", "1.9-1.12", "1.18", "pre-1.18" };

        static bool SecondaryCachesLoaded;

        public string Category { get; private set; }
        public string Version { get; private set; }

        public Dictionary<string, int> Ranks = new ();
        public List<Run> Runs = new ();

        //private readonly LeaderboardSheet sheet;

        static bool CachedChallengesLoaded;
        static bool CachedHistoryLoaded;

        public static bool NickNamesLoaded { get; private set; }
        public static (string category, string version) Current => (Tracker.Category.Name, Tracker.Category.CurrentMajorVersion);
        public static bool IdentityAlreadyRequested(string name) => RequestedIdentities.Contains(name);
        public static bool IsRunner(Uuid player, string name = null) => AllRunners.Contains(player) || AllRunnerNames.Contains(name);

        public static bool IsLiveAvailable(string category, string version)
        {
            if (category is "HHHAA")
                return History is not null;
            return LiveBoards.Contains((category, version));
        }

        public static void Initialize()
        {
            foreach (string version in AAVersions)
                TryLoadCached("All Advancements", version, out _);
            foreach (string version in AAVersions)
                TryLoadCached("All Advancements", version, out _);

            TryLoadCachedSpeedrunDotComRecord(true, false, "1.16", out _);
            TryLoadCachedSpeedrunDotComRecord(false, false, "1.16", out _);
            TryLoadCachedSpeedrunDotComRecord(false, true, "1.16", out _);
            TryLoadCachedChallenges();
            UpdateMostConcurrentRecords();
            UpdateMostConsecutiveRecords();
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

        public static void RefreshSrc(string category = null, string version = null)
        {
            NetRequest.ClearHistory();
            Player.IdentitiesAlreadyRequested.Clear();
            Player.NamesAlreadyRequested.Clear();
            Badge.HoverTimer.Reset();
            NickNamesLoaded = false;
            if (category is null)
            {
                SrcLeaderboardRequest.DownloadedLeaderboards.Clear();
                LiveBoards.Clear();
            }
            else
            {
                SrcLeaderboardRequest.DownloadedLeaderboards.Remove((category, version));
                LiveBoards.Remove((category, version));
            }
        }

        public static string GuidanceHeader(string caterogy, string version)
        {
            //1.16 aa has its own separate page
            return (caterogy is "All Advancements" && version is "1.16") || caterogy is "All Blocks"
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
            if (category is "Challenge")
                return $"leaderboard_challenges";
            return version is "1.16" ? "leaderboard_aa_primary" : "leaderboard_aa_others";
        }

        public Leaderboard(string category, string version)
        {
            this.Category = category;
            this.Version = version;
        }

        public Leaderboard(LeaderboardSheet sheet, string category, string version)
        {
            this.Category = category;
            this.Version = version;
            lock (this.Ranks)
            {
                if (category is "Hardcore No Reset")
                {
                    for (int i = 2; i < sheet.Rows.Length; i++)
                    {
                        if (HardcoreStreak.TryParse(sheet, i, this.Version, out HardcoreStreak run))
                        {
                            //int rank = i - 1;
                            int rank = run.BestStreak switch {
                                >= 100 => 1,
                                >= 50 => 2,
                                _ => int.MaxValue
                            };
                            this.AddRun(run, rank);

                            if (rank is 1)
                                HundredHardcoreCompletions.Add(run);
                        }
                    }
                }
                else if (category is "All Versions")
                {
                    for (int i = 2; i < sheet.Rows.Length; i++)
                    {
                        if (AllVersionsRun.TryParse(sheet, i, this.Version, out Run run))
                        {
                            int rank = i - 1;
                            this.AddRun(run, rank);
                        }
                    }
                }
                else
                {
                    for (int i = 2; i < sheet.Rows.Length; i++)
                    {
                        if (Run.TryParse(sheet, i, this.Version, out Run run))
                        {
                            int rank = i - 1;
                            this.AddRun(run, rank);
                        }
                    }

                    if (category is "1K No Reset")
                    {
                        foreach (Run run in this.Runs)
                        {
                            run.Comment = "1K No Reset";
                        }
                    }
                }
            }
        }

        public Leaderboard(LeaderboardSrcJson json, string category, string version)
        {
            this.Category = category;
            this.Version = version;
            lock (this.Ranks)
            {
                for (int i = 0; i < json.Runs.Count; i++)
                {
                    Run run = json.Runs[i];
                    int rank = i + 1;
                    this.AddRun(run, rank);
                }
            }
        }

        public void AddRun(Run run, int rank)
        {
            this.Runs.Add(run);
            this.Ranks[run.Runner.ToLower()] = rank;

            string realName = GetRealName(run.Runner);
            _= AllRunnerNames.Add(realName);
            _= AllRunnerNames.Add(run.Runner);
            if (Player.TryGetUuid(realName, out Uuid id))
                _= AllRunners.Add(id);
        }

        public static bool TryGet(string category, string version, out Leaderboard leaderboard)
        {
            if (category is "HHHAA")
            {
                leaderboard = HalfHeartHardcoreCompletions;
                return leaderboard is not null;
            }

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

        public static bool SyncSheetLeaderboard(string sheetId, string pageId, string csv)
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
                versions = new() { "1.16" };
            else if (sheetId is Paths.Web.ABSheet && pageId is Paths.Web.ABPage16)
                versions = new() { "1.16" };
            else if (pageId is Paths.Web.ABPage18)
                versions = new() { "1.18" };
            else if (pageId is Paths.Web.ABPage19)
                versions = new() { "1.19" };
            else if (pageId is Paths.Web.ABPage20)
                versions = new() { "1.20" };
            else
                versions = new() { "1.21", "1.20.5", "1.20", "1.19", "1.18", "1.17", "1.15", "1.14", "1.13", "1.12", "1.11", "1.6" };

            //parse all the leaderboards
            foreach (string version in versions)
            {
                if (LeaderboardSheet.TryParse(csv, GetKey(category, version), GuidanceHeader(category, version), out sheet))
                {
                    AllBoards[(category, version)] = new Leaderboard(sheet, category, version);
                    LiveBoards.Add((category, version));
                }
            }
            UpdateMostConcurrentRecords();
            sheet?.SaveToCache();
            return sheet is not null;
        }

        public static bool SyncChallengeLeaderboards(string csv)
        {
            LeaderboardSheet sheet = null;
            string[] challenges = new string[] { "Hardcore No Reset", "1K No Reset", "All Items", "All Versions" };
            foreach (string challenge in challenges)
            {
                if (LeaderboardSheet.TryParse(csv, GetKey("Challenge", ""), challenge.ToLower(), out sheet))
                {
                    AllBoards[(challenge, "1.16")] = new Leaderboard(sheet, challenge, "1.16");
                    LiveBoards.Add((challenge, "1.16"));
                }
            }
            sheet?.SaveToCache();
            return sheet is not null;
        }

        public static bool SyncHistory(string csv, bool save)
        {
            if (LeaderboardSheet.TryParse(csv, "history_aa_1.16", null, out LeaderboardSheet sheet))
            {
                History = sheet;
                AALinks.Clear();
                var hhhRuns = new List<Run>();
                for (int i = 1; i < History.Rows.Length; i++)
                {
                    if (!History.TryGetRunner(i, out string runner))
                        continue;
                    if (string.IsNullOrEmpty(runner))
                        continue;
                    if (!History.TryGetDate(i, out DateTime date))
                        continue;

                    if (History.TryGetLink(i, out string link))
                        AALinks[(runner, date)] = link;

                    if (History.TryGetComment(i, out string comment) && comment is "Modded (HHH mod)")
                    {
                        if (!History.TryGetIgt(i, out TimeSpan igt))
                            continue;
                        History.TryGetIgt(i, out TimeSpan rta);

                        hhhRuns.Add(new Run() { 
                            Runner = runner,
                            Date = date,
                            InGameTime = igt,
                            RealTime = rta,
                            Link = link
                        });
                    }
                }
                hhhRuns = hhhRuns.OrderBy(x => x.InGameTime).ToList();
                if (hhhRuns.Count > 0)
                {
                    HalfHeartHardcoreCompletions = new Leaderboard("HHHAA", "1.16");
                    for (int i = 0; i < hhhRuns.Count; i++)
                        HalfHeartHardcoreCompletions.AddRun(hhhRuns[i], i + 1);
                }
            }
            if (save)
                sheet?.SaveToCache();
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

        internal static bool SyncSpeedrunDotComLeaderboard(string json, string category, string version)
        {
            if (LeaderboardSrcJson.TryParse(json, version, out LeaderboardSrcJson valid))
            {
                AllBoards[(category, version)] = new Leaderboard(valid, category, version);
                LiveBoards.Add((category, version));
                return true;
            }
            return false;
        }

        public static bool SyncSpeedrunDotComRecord(string jsonString, bool rsg, bool aa)
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
                    AnyRsgRunner = runner;
                    AnyRsgInGameTime = igt;
                    AnyRsgRealTime = rta;
                }
                else if (aa)
                {
                    AASsgRunner = runner;
                    AASsgInGameTime = igt;
                    AASsgRealTime = rta;
                }
                else
                {
                    AnySsgRunner = runner;
                    AnySsgInGameTime = igt;
                    AnySsgRealTime = rta;
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

        public static void SaveSpeedrunDotComLeaderboardToCache(string jsonString, string category, string version)
        {
            try
            {
                //cache leaderboard so it loads instantly next launch
                //overwrite to keep leaderboard up to date
                Directory.CreateDirectory(Paths.System.LeaderboardsFolder);
                string path = Paths.System.SpeedrunDotComLeaderboardFile(category, version);
                File.WriteAllText(path, jsonString);
            }
            catch
            {
                //couldn't save file. ignore and move on
            }
        }

        public static void SaveSpeedrunDotComRecordToCache(string jsonString, bool rsg, bool aa)
        {
            try
            {
                //cache leaderboard so it loads instantly next launch
                //overwrite to keep leaderboard up to date
                Directory.CreateDirectory(Paths.System.LeaderboardsFolder);
                string path = Paths.System.SpeedrunDotComRecordFile(rsg, aa, "1.16");
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
            if (category.ToLower().Contains("any%"))
            {
                return TryLoadCachedSrc(category, version, out leaderboard);
            }
            else if (category is "1K No Reset")
            {
                if (TryLoadCachedChallenges())
                {
                    leaderboard = AllBoards[(category, "1.16")];
                    return true;
                }
                return false;
            }

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

        public static bool TryLoadCachedHistory()
        {
            if (CachedHistoryLoaded)
                return false;

            CachedHistoryLoaded = true;
            string leaderboardFile = Paths.System.HistoryFile;
            if (File.Exists(leaderboardFile))
            {
                try
                {
                    string csv = File.ReadAllText(leaderboardFile);
                    return SyncHistory(csv, false);
                }
                catch
                {
                    //couldn't read cached history, move on 
                }
            }
            return false;
        }

        public static bool TryLoadCachedChallenges()
        {
            if (CachedChallengesLoaded)
                return false;

            CachedChallengesLoaded = true;
            string leaderboardFile = Paths.System.ChallengesFile;
            if (File.Exists(leaderboardFile))
            {
                try
                {
                    string csv = File.ReadAllText(leaderboardFile);
                    return SyncChallengeLeaderboards(csv);
                }
                catch
                {
                    //couldn't read cached history, move on 
                }
            }
            return false;
        }

        public static bool TryLoadCachedSrc(string category, string version, out Leaderboard leaderboard)
        {
            leaderboard = null;
            string leaderboardFile = Paths.System.SpeedrunDotComLeaderboardFile(category, version);
            if (File.Exists(leaderboardFile))
            {
                try
                {
                    string json = File.ReadAllText(leaderboardFile);
                    if (LeaderboardSrcJson.TryParse(json, version, out LeaderboardSrcJson sheet))
                        AllBoards[(category, version)] = leaderboard = new Leaderboard(sheet, category, version);
                    else
                        AllBoards[(category, version)] = null;
                }
                catch
                {
                    //couldn't read cached leaderboard, move on 
                }
            }
            return leaderboard is not null;
        }

        private static bool TryLoadCachedSpeedrunDotComRecord(bool rsg, bool aa, string version, out string jsonString)
        {
            jsonString = string.Empty;
            string leaderboardFile = Paths.System.SpeedrunDotComRecordFile(rsg, aa, version);
            if (File.Exists(leaderboardFile))
            {
                try
                {
                    jsonString = File.ReadAllText(leaderboardFile);
                    SyncSpeedrunDotComRecord(jsonString, rsg, aa);
                }
                catch
                {
                    //couldn't read cached leaderboard, move on 
                }
            }
            return jsonString is not null;
        }

        private static void UpdateMostConcurrentRecords()
        {
            Dictionary<string, List<Run>> recordHolders = new ();
            foreach (string version in AAVersions)
            {
                if (!TryGetWorldRecord("All Advancements", version, out Run wr))
                    continue;

                if (!recordHolders.TryGetValue(wr.Runner, out List<Run> records))
                    recordHolders[wr.Runner] = records = new List<Run>();
                records.Add(wr);
            }

            Version mostLatestVersion = default;
            int mostRecordsCount = 0;
            foreach (KeyValuePair<string, List<Run>> recordHolder in recordHolders)
            {
                int count = recordHolder.Value.Count;
                Run newestVersion = recordHolder.Value.FirstOrDefault();
                
                bool foundNewLeader = count > mostRecordsCount
                    || (count == mostRecordsCount && newestVersion.GameVersion > mostLatestVersion);
                
                if (foundNewLeader)
                {
                    mostRecordsCount = count;
                    RunnerWithMostConcurrentRecords = newestVersion.Runner;
                    mostLatestVersion = newestVersion.GameVersion;
                }
            }
            ListOfMostConcurrentRecords.Clear();
            if (recordHolders.TryGetValue(RunnerWithMostConcurrentRecords, out List<Run> runs))
                ListOfMostConcurrentRecords.AddRange(runs);
        }

        private static void UpdateMostConsecutiveRecords()
        {
            TryLoadCachedHistory();

            if (History is null || !History.Rows.Any())
                return;

            TimeSpan wr = TimeSpan.MaxValue;
            var streaks = new List<(string runner, int records, DateTime date)>();
            string previousRecordHolder = null;

            (string runner, int records, DateTime date) current = default;
            for (int row = 1; row < History.Rows.Length; row++)
            {
                if (!Run.TryParse(History, row, "1.16", out Run run))
                    continue;
                if (run.InGameTime > wr)
                    continue;

                wr = run.InGameTime;

                if (current == default || current.runner != run.Runner)
                {
                    current = new(run.Runner, 1, run.Date);
                    streaks.Add(current);
                }

                if (run.Runner == previousRecordHolder)
                {
                    current = new(current.runner, current.records + 1, current.date);
                    streaks[streaks.Count - 1] = current;
                }
                previousRecordHolder = run.Runner;
            }

            (string runner, int records, DateTime date) most = default;
            foreach ((string runner, int records, DateTime date) streak in streaks)
            {
                if (streak.records > most.records)
                    most = streak;
            }

            RunnerWithMostConsecutiveRecords = most.runner;
            MostConsecutiveRecordsCount = most.records;
        }
    }
}

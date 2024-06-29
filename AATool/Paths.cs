using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Net;
using AATool.UI.Screens;
using AATool.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using static System.Environment;

namespace AATool
{
    public static class Paths
    {
        public static bool TryGetAllFiles(string path, string pattern, SearchOption search, out IEnumerable<string> files)
        {
            files = default;
            if (Directory.Exists(path))
            {
                try
                {
                    files = Directory.EnumerateFiles(path, pattern, search);
                    return true;
                }
                catch (ArgumentException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (SecurityException) { }
            }
            return false;
        }

        public static class System
        {
            //constant settings paths
            public const string ConfigFolder = "config/";
            public const string LegacySettingsFolder = "settings/";
            public const string ArchivedConfigFolder = "config/legacy_settings_(unused)/";
            public const string NotesFolder  = "notes/";

            public const string CacheFolder = "assets/cache/";
            public const string LeaderboardsFolder = CacheFolder + "leaderboards/";
            public const string BlockChecklistsFolder = CacheFolder + "block_checklists/";
            public const string ProfilePicturesCacheFolder = CacheFolder + "runner_profiles/pictures/";
            public const string ProfileDetailsCacheFolder = CacheFolder + "runner_profiles/details/";
            //remote world temp folder
            public const string SftpWorldsFolder = CacheFolder + "sftp_worlds/";

            //constant asset paths
            public const string DataFolder        = "data/";
            public const string LogsFolder        = "logs/";
            public const string AssetsFolder      = "assets/";
            public const string ObjectivesFolder  = AssetsFolder  + "objectives/";
            public const string ViewsFolder       = AssetsFolder  + "views/";
            public const string TemplatesFolder   = AssetsFolder  + "templates";
            public const string SpritesFolder     = AssetsFolder  + "sprites/";
            public const string FontsFolder       = AssetsFolder  + "fonts/";
            public const string AvatarCacheFolder = SpritesFolder + "/global/avatar_cache";
            public const string CreditsFolder     = AssetsFolder  + "credits/";
            public const string WinformsAssets    = SpritesFolder + "winforms/";

            public const string MainIcon = "assets/icons/aatool.ico";
            public const string UpdateIcon = "assets/icons/aaupdate.ico";

            //constant urls
            public const string UpdateExecutable = "AAUpdate.exe";

            //dependant paths
            public static string ObjectiveFolder => Path.Combine(ObjectivesFolder, Config.Tracking.GameVersion);
            public static string AdvancementsFolder => Path.Combine(ObjectiveFolder, "advancements/");
            public static string AchievementsFile => Path.Combine(ObjectiveFolder, "achievements.xml");
            public static string DeathMessagesFile => Path.Combine(ObjectiveFolder, "deaths.xml");
            public static string ArmorTrimsFile => Path.Combine(ObjectiveFolder, "trims.xml");
            public static string PotionsFile => Path.Combine(ObjectiveFolder, "potions.xml");

            //file getters
            public static string CrashLogFile => Path.Combine(LogsFolder, $"crash_report_{DateTime.Now:yyyy_M_dd_h_mm_ss}.txt");
            public static string CreditsFile => Path.Combine(CreditsFolder, "credits.xml");

            public static string HistoryFile =>
                Path.Combine(LeaderboardsFolder, $"history_aa_1.16.csv");

            public static string ChallengesFile =>
                Path.Combine(LeaderboardsFolder, $"leaderboard_challenges.csv");

            public static string SupportersFile =>
                Path.Combine(LeaderboardsFolder, "supporters.csv");

            public static string LeaderboardFile(string fileName) =>
                Path.Combine(LeaderboardsFolder, $"{fileName}.csv");

            public static string BlockChecklistFile(int instance, string worldName)
            {
                return instance < 1
                    ? Path.Combine(BlockChecklistsFolder, $"{worldName}.txt")
                    : Path.Combine(BlockChecklistsFolder, $"instance_{instance}-{worldName}.txt");
            }

            public static string SpeedrunDotComLeaderboardFile(string category, string version)
            {
                return Path.Combine(LeaderboardsFolder, $"speedrundotcom_leaderboard_{category}_{version}.json");
            }

            public static string SpeedrunDotComRecordFile(bool rsg, bool aa, string version)
            {
                if (aa)
                    return Path.Combine(LeaderboardsFolder, $"aa_wr_ssg_{version}.txt");

                return rsg
                    ? Path.Combine(LeaderboardsFolder, $"any_percent_wr_rsg_{version}.txt")
                    : Path.Combine(LeaderboardsFolder, $"any_percent_wr_ssg_{version}.txt");
            }

            public static string SpeedrunDotComProfilePicture(string id) =>
                Path.Combine(ProfilePicturesCacheFolder, $"{id}.png");

            public static string SpeedrunDotComProfileJson(string idOrName) =>
                Path.Combine(ProfileDetailsCacheFolder, $"{idOrName}.json");
        }

        public static class Saves
        {
            public const string AppDataShortcut = "%AppData%\\Roaming";
            private static readonly string AppDataFolderPath = GetFolderPath(SpecialFolder.ApplicationData);

            public static string CurrentFolder()
            {
                if (Config.Tracking.UseSftp)
                    return System.SftpWorldsFolder;

                return Tracker.Source is TrackerSource.CustomSavesPath
                    ? Config.Tracking.CustomSavesPath.Value.Replace(AppDataShortcut, AppDataFolderPath)
                    : ActiveInstance.SavesPath;
            }

            public static string CurrentPracticeSavesFolder()
            {
                if (Config.Tracking.UseSftp)
                    return string.Empty;

                return Tracker.Source is TrackerSource.CustomSavesPath
                    ? string.Empty
                    : ActiveInstance.PracticeSavesPath;
            }

            public static string DefaultAppDataSavesPath => Path.Combine(
                GetFolderPath(SpecialFolder.ApplicationData),
                ".minecraft",
                "saves");

            public static bool MightBeWorldFolder(DirectoryInfo folder)
            {
                return File.Exists(Path.Combine(folder.FullName, "level.dat"))
                    || Directory.Exists(Path.Combine(folder.FullName, "advancements"))
                    || Directory.Exists(Path.Combine(folder.FullName, "stats"));
            }

            public static DirectoryInfo MostRecentlyWritten(DirectoryInfo a, DirectoryInfo b)
            {
                if (a is null)
                    return b;
                if (b is null)
                    return a;
                return a.LastWriteTimeUtc > b.LastWriteTimeUtc ? a : b;
            }
        }

        public static class Web
        {
            public const string LatestRelease = "https://github.com/DarwinBaker/AATool/releases/latest";
            public const string ObsHelp       = "https://github.com/DarwinBaker/AATool/blob/main/info/obs.md";
            public const string PatreonFull   = "https://www.patreon.com/_ctm";
            public const string PatreonShort  = "Patreon.com/_CTM";

            public const string PayPal = "https://www.paypal.com/donate/?hosted_button_id=EN29468P8CY24";

            public const string AASheet = "107ijqjELTQQ29KW4phUmtvYFTX9-pfHsjb18TKoWACk";
            public const string AAPage16 = "1706556435";
            public const string AAPageOthers = "1283472797";

            public const string ABSheet = "1RnN6lE3yi5S_5PBuxMXdWNvN3HayP3054M3Qud_p9BU";
            public const string ABPage21 = "27712269";
            public const string ABPage20 = "1664598957";
            public const string ABPage19 = "1912774860";
            public const string ABPage18 = "1706556435";
            public const string ABPage16 = "1572184167";
            public const string ABPageChallenges = "2045031868";

            public const string SupporterSheet = "1Vj1e2kREWuw8XzMu6OazHmbvC-QXCVBH08CaQXnrOD4";
            public const string NicknameSheet = "16VS6VkitZdyrfVAFd-UdkVSrXO0nhdMyNeueIFoqvZY";
            public const string PrimaryAAHistory = "735237004";
            
            public const string AnyRsgRecord = "https://www.speedrun.com/api/v1/leaderboards/j1npme6p/category/mkeyl926?top=1&embed=players&var-jlzkwql2=mln68v0q&var-r8rg67rn=21d4zvp1";
            public const string AnySsgRecord = "https://www.speedrun.com/api/v1/leaderboards/j1npme6p/category/mkeyl926?top=1&embed=players&var-wl33kewl=4qye4731&var-r8rg67rn=klrzpjo1";
            public const string AASsgRecord  = "https://www.speedrun.com/api/v1/leaderboards/j1npme6p/category/xk9gz16d?top=1&embed=players&var-38do09zl=5q8rd731&var-r8rg67rn=klrzpjo1";

            public static string GetUuidUrl(string name) => 
                $"https://api.mojang.com/users/profiles/minecraft/{name}";

            public static string GetNameUrl(string uuid) => 
                $"https://api.mojang.com/user/profile/{uuid.Replace("-", "")}";

            public static string GetAvatarUrlFallback(Uuid uuid, int size) =>
                $"https://crafatar.com/avatars/{uuid}?size={size}&overlay=true";

            public static string GetAvatarUrl(Uuid uuid, int size) =>
                $"https://minotar.net/helm/{uuid.ShortString}/{size}";

            public static string GetAvatarUrl(string name, int size) =>
                $"https://minotar.net/helm/{name.Trim()}/{size}";

            public static string GetSpreadsheetUrl(string sheet, string page) =>
                $"https://docs.google.com/spreadsheets/d/{sheet}/export?gid={page}&format=csv";

            public static string GetSpeedrunDotComProfileUrl(string id) =>
                $"https://www.speedrun.com/api/v1/users/{id}";

            public static string GetSpeedrunDotComPictureUrl(string id) =>
                $"https://www.speedrun.com/static/user/{id}/image.png";
            
            public static string GetAnyPercentRecordUrl(bool rsg) => rsg ? AnyRsgRecord : AnySsgRecord;
        }
    }
}

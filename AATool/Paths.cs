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

            //remote world temp folder
            public const string CacheFolder = "assets/cache/";
            public const string SftpWorldsFolder = CacheFolder + "sftp_worlds/";
            public const string LeaderboardsFolder = CacheFolder + "leaderboards/";
            public const string BlockChecklistsFolder = CacheFolder + "block_checklists/";

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

            public static string LeaderboardFile(string fileName) => 
                Path.Combine(LeaderboardsFolder, $"{fileName}.csv");

            public static string BlockChecklistFile(int instance, string worldName)
            {
                return instance < 1
                    ? Path.Combine(BlockChecklistsFolder, $"{worldName}.txt")
                    : Path.Combine(BlockChecklistsFolder, $"instance_{instance}-{worldName}.txt");
            }

            public static string AnyPercentRecordFile(bool rsg, string version)
            {
                return rsg
                    ? Path.Combine(LeaderboardsFolder, $"any_percent_wr_rsg_{version}.txt")
                    : Path.Combine(LeaderboardsFolder, $"any_percent_wr_ssg_{version}.txt");
            }
        }

        public static class Saves
        {
            public const string AppDataShortcut = "%AppData%\\Roaming";

            public static string CurrentFolder()
            {
                if (Config.Tracking.UseSftp)
                    return System.SftpWorldsFolder;

                return Tracker.Source is TrackerSource.CustomSavesPath
                    ? Config.Tracking.CustomSavesPath.Value.Replace(AppDataShortcut, GetFolderPath(SpecialFolder.ApplicationData))
                    : ActiveInstance.SavesPath;
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

            public const string AASheet = "107ijqjELTQQ29KW4phUmtvYFTX9-pfHsjb18TKoWACk";
            public const string AAPage16 = "1706556435";
            public const string AAPageOthers = "1283472797";

            public const string ABSheet = "1RnN6lE3yi5S_5PBuxMXdWNvN3HayP3054M3Qud_p9BU";
            public const string ABPage19 = "1912774860";
            public const string ABPage18 = "1706556435";
            public const string ABPage16 = "1572184167";

            public const string NicknameSheet = "1j2APgxS_En7em5lcVF2OWjEvsUY2DHVX4QvdVGhSR_o";
            public const string PrimaryAAHistory = "735237004";
            
            public const string AnyRsgRecord = "https://www.speedrun.com/api/v1/leaderboards/j1npme6p/category/mkeyl926?top=1&embed=players&var-jlzkwql2=mln68v0q&var-r8rg67rn=21d4zvp1";
            public const string AnySsgRecord = "https://www.speedrun.com/api/v1/leaderboards/j1npme6p/category/mkeyl926?top=1&embed=players&var-wl33kewl=4qye4731&var-r8rg67rn=klrzpjo1";

            public static string GetUuidUrl(string name) => 
                $"https://minecraft-api.com/api/uuid/{name}";

            public static string GetNameUrl(string uuid) => 
                $"https://minecraft-api.com/api/pseudo/{uuid.Replace("-", "")}";

            public static string GetAvatarUrlFallback(Uuid uuid, int size) =>
                $"https://crafatar.com/avatars/{uuid}?size={size}&overlay=true";

            public static string GetAvatarUrl(Uuid uuid, int size) =>
                $"https://minotar.net/helm/{uuid.ShortString}/{size}";

            public static string GetAvatarUrl(string name, int size) =>
                $"https://minotar.net/helm/{name.Trim()}/{size}";

            public static string GetSpreadsheetUrl(string sheet, string page) =>
                $"https://docs.google.com/spreadsheets/d/{sheet}/export?gid={page}&format=csv";

            public static string GetAnyPercentRecordUrl(bool rsg) => rsg ? AnyRsgRecord : AnySsgRecord;
        }
    }
}

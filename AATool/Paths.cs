using AATool.Configuration;
using AATool.Data.Categories;
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

            public const string MainIcon = "assets/icons/aatool.ico";
            public const string UpdateIcon = "assets/icons/aaupdate.ico";

            //constant urls
            public const string UpdateExecutable = "AAUpdate.exe";

            //dependant paths
            public static string ObjectiveFolder => Path.Combine(ObjectivesFolder, Config.Tracking.GameVersion);
            public static string AdvancementsFolder => Path.Combine(ObjectiveFolder, "advancements/");
            public static string AchievementsFile => Path.Combine(ObjectiveFolder, "achievements.xml");
            public static string StatisticsFile => Path.Combine(ObjectiveFolder, "statistics.xml");
            public static string DeathMessagesFile => Path.Combine(ObjectiveFolder, "deaths.xml");
            public static string PotionsFile => Path.Combine(ObjectiveFolder, "potions.xml");

            //file getters
            public static string CrashLogFile => Path.Combine(LogsFolder, $"crash_report_{DateTime.Now:yyyy_M_dd_h_mm_ss}.txt");
            public static string CreditsFile => Path.Combine(CreditsFolder, "credits.xml");

            public static string LeaderboardFile(string fileName) => 
                Path.Combine(LeaderboardsFolder, $"{fileName}.csv");

            public static string BlockChecklistFile(string instance, string worldName) => 
                Path.Combine(BlockChecklistsFolder, $"{instance}{worldName}.txt");
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

            public const string LeaderboardSpreadsheet = "107ijqjELTQQ29KW4phUmtvYFTX9-pfHsjb18TKoWACk";
            public const string NicknameSpreadsheet = "1j2APgxS_En7em5lcVF2OWjEvsUY2DHVX4QvdVGhSR_o";
            public const string PrimaryVersionBoard = "1706556435";
            public const string OtherVersionsBoard = "1283472797";

            public static string GetUuidUrl(string mojangName) => 
                $"https://api.mojang.com/users/profiles/minecraft/{mojangName}";

            public static string GetNameUrl(string uuid) => 
                $"https://api.mojang.com/user/profiles/{uuid}/names";

            public static string GetAvatarUrl(string uuid, int size = 16) => 
                $"https://crafatar.com/avatars/{uuid}?size={size}&overlay=true";

            public static string GetSpreadsheetUrl(string sheet, string page) =>
                $"https://docs.google.com/spreadsheets/d/{sheet}/export?gid={page}&format=csv";
        }
    }
}

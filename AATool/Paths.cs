using AATool.Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace AATool
{
    public static class Paths
    {
        //constant settings paths
        public const string DIR_SETTINGS            = "settings/";
        public const string DIR_FAVORITES           = DIR_SETTINGS + "favorites";
        public const string DIR_NOTES               = "notes/";

        //constant asset paths
        public const string DIR_ASSETS              = "assets/";
        public const string DIR_GAME_VERSIONS       = DIR_ASSETS   + "game_versions/";
        public const string DIR_UI_CONTROLS         = DIR_ASSETS   + "ui/controls";
        public const string DIR_THEMES              = DIR_ASSETS + "ui/themes/";
        public const string DIR_GRAPHICS            = DIR_ASSETS   + "graphics/";
        public const string DIR_SPRITES             = DIR_GRAPHICS + "sprites/";
        public const string DIR_FONTS               = DIR_GRAPHICS + "fonts/";
        public const string DIR_SKIN_CACHE          = DIR_SPRITES  + "skin_cache";
        public const string DIR_CREDITS             = DIR_ASSETS   + "credits/";
        public const string DIR_LOGS                = "logs/";

        //constant urls
        public const string URL_GITHUB_LATEST       = "https://github.com/DarwinBaker/AATool/releases/latest";
        public const string URL_HELP_OBS            = "https://github.com/DarwinBaker/AATool/blob/master/info/obs.md";
        public const string URL_PATREON             = "https://www.patreon.com/_ctm";
        public const string URL_PATREON_FRIENDLY    = "Patreon.com/_CTM";
        public const string URL_API_MC_UUID         = "https://api.mojang.com/users/profiles/minecraft/";
        public const string URL_API_MC_NAME         = "https://api.mojang.com/user/profiles/";

        //getters for version-dependant folders
        public static string CurrentVersionFolder   => Path.Combine(DIR_GAME_VERSIONS, Config.Tracker.GameVersion);
        public static string AdvancementsFolder     => Path.Combine(CurrentVersionFolder, "advancements/");
        public static string LayoutsFolder          => Path.Combine(CurrentVersionFolder, "layouts/");

        //getters for version-dependant files
        public static string AchievementsFile => Path.Combine(CurrentVersionFolder, "achievements.xml");
        public static string StatisticsFile   => Path.Combine(CurrentVersionFolder, "statistics.xml");
        public static string PotionsFile      => Path.Combine(CurrentVersionFolder, "potions.xml");
        public static string CreditsFile      => Path.Combine(DIR_CREDITS, "credits.xml");

        public static string CrashLogFile     => 
            Path.Combine(DIR_LOGS, $"crash_report_{DateTime.Now:yyyy_M_dd_h_mm_ss}.txt");

        public static IEnumerable<string> AdvancementFiles 
            => Directory.EnumerateFiles(AdvancementsFolder, "*.xml", SearchOption.TopDirectoryOnly);

        //getters for ui stuff
        public static string GetLayoutFor(string name, string variant = null)
        {
            return string.IsNullOrEmpty(variant)
                ? Path.Combine(LayoutsFolder, $"{name}.xml")
                : Path.Combine(LayoutsFolder, $"{name}_{variant}.xml");
        }


        public static string GetStyleFor(string name) => 
            Path.Combine(DIR_THEMES, name + ".xml");

        //getters for urls
        public static string GetUrlForPlayerHead(string uuid, int resolution = 16) => 
            $"https://crafatar.com/avatars/{uuid}?size={resolution}&overlay=true";
        public static string GetUrlForUUID(string mojangName) =>
            URL_API_MC_UUID + mojangName;
        public static string GetUrlForName(string uuid) =>
            URL_API_MC_NAME + uuid + "/names";
    }
}

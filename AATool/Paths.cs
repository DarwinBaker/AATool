using AATool.Settings;
using System.Collections.Generic;
using System.IO;

namespace AATool
{
    public static class Paths
    {
        //constant settings paths
        public const string DIR_SETTINGS        = "settings/";
        public const string DIR_FAVORITES       = DIR_SETTINGS + "favorites";

        //constant asset paths
        public const string DIR_ASSETS          = "assets/";
        public const string DIR_GAME_VERSIONS   = DIR_ASSETS   + "game_versions/";
        public const string DIR_UI              = DIR_ASSETS   + "ui/";
        public const string DIR_GRAPHICS        = DIR_ASSETS   + "graphics/";
        public const string DIR_TEXTURES        = DIR_GRAPHICS + "sprites/";
        public const string DIR_FONTS           = DIR_GRAPHICS + "fonts/";
        
        //getters for version-dependant folders
        public static string VersionFolder      => Path.Combine(DIR_GAME_VERSIONS, TrackerSettings.Instance.GameVersion);
        public static string AdvancementsFolder => Path.Combine(VersionFolder, "advancements/");
        public static string LayoutsFolder      => Path.Combine(VersionFolder, "layouts/");

        //getters for version-dependant files
        public static string StatisticsFile     => Path.Combine(VersionFolder, "statistics.xml");
        public static string PotionsFile        => Path.Combine(VersionFolder, "potions.xml");
        public static IEnumerable<string> AdvancementFiles => Directory.EnumerateFiles(AdvancementsFolder, "*.xml", SearchOption.TopDirectoryOnly);
        public static string GetLayoutFor(string name)     => Path.Combine(LayoutsFolder, name + ".xml");
    }
}

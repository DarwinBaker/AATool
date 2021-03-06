﻿using AATool.Settings;
using System.Collections.Generic;
using System.IO;

namespace AATool
{
    public static class Paths
    {
        //constant settings paths
        public const string DIR_SETTINGS        = "settings/";
        public const string DIR_FAVORITES       = DIR_SETTINGS + "favorites";
        public const string DIR_NOTES           = "notes/";

        //constant asset paths
        public const string DIR_ASSETS          = "assets/";
        public const string DIR_GAME_VERSIONS   = DIR_ASSETS   + "game_versions/";
        public const string DIR_UI_CONTROLS     = DIR_ASSETS   + "ui/controls";
        public const string DIR_GRAPHICS        = DIR_ASSETS   + "graphics/";
        public const string DIR_TEXTURES        = DIR_GRAPHICS + "sprites/";
        public const string DIR_FONTS           = DIR_GRAPHICS + "fonts/";
        public const string DIR_CREDITS         = DIR_ASSETS   + "credits/";

        //getters for version-dependant folders
        public static string CurrentVersionFolder   => Path.Combine(DIR_GAME_VERSIONS, TrackerSettings.Instance.GameVersion);
        public static string AdvancementsFolder     => Path.Combine(CurrentVersionFolder, "advancements/");
        public static string LayoutsFolder          => Path.Combine(CurrentVersionFolder, "layouts/");

        //getters for version-dependant files
        public static string AchievementsFile       => Path.Combine(CurrentVersionFolder, "achievements.xml");
        public static string StatisticsFile         => Path.Combine(CurrentVersionFolder, "statistics.xml");
        public static string PotionsFile            => Path.Combine(CurrentVersionFolder, "potions.xml");
        public static string CreditsFile            => Path.Combine(DIR_CREDITS, "credits.xml");
        public static IEnumerable<string> AdvancementFiles => Directory.EnumerateFiles(AdvancementsFolder, "*.xml", SearchOption.TopDirectoryOnly);
        public static string GetLayoutFor(string name)     => Path.Combine(LayoutsFolder, name + ".xml");
    }
}

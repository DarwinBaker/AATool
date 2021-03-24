using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class TrackerSettings : SettingsGroup
    {
        public static TrackerSettings Instance = new TrackerSettings();

        public static HashSet<string> SupportedVersions;

        public const string GAME_VERSION       = "game_version";
        public const string REFRESH_INTERVAL   = "refresh_interval_ms";
        public const string USE_DEFAULT_PATH   = "use_default_saves_folder";
        public const string AUTO_GAME_VERSION  = "auto_game_version";
        public const string CUSTOM_FOLDER      = "custom_saves_folder";
        public const string SUPPORTED_VERSIONS = "supported_versions";

        public string GameVersion       { get => Get<string>(GAME_VERSION);      private set => GameVersion = value; }
        public int RefreshInterval      { get => Get<int>(REFRESH_INTERVAL);     set => Set(REFRESH_INTERVAL, value); }
        public bool UseDefaultPath      { get => Get<bool>(USE_DEFAULT_PATH);    set => Set(USE_DEFAULT_PATH, value); }
        public bool AutoDetectVersion   { get => Get<bool>(AUTO_GAME_VERSION);   set => Set(AUTO_GAME_VERSION, value); }
        public string CustomPath        { get => Get<string>(CUSTOM_FOLDER);     set => Set(CUSTOM_FOLDER, value); }
        
        public static bool IsPostExplorationUpdate { get; private set; }
        public static bool IsPreExplorationUpdate => !IsPostExplorationUpdate;
        public static string DefaultSavesFolder    => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves");
        public string SavesFolder                  => UseDefaultPath ? DefaultSavesFolder : CustomPath;

        public void TrySetGameVersion(string version)
        {
            if (SupportedVersions?.Contains(version) ?? false)
                Set(GAME_VERSION, version);
            string formatted = new string(GameVersion.Where(c => char.IsDigit(c)).ToArray()).Insert(1, ".");
            double.TryParse(formatted, out double number);
            IsPostExplorationUpdate = 1.12 <= number;
        }

        private TrackerSettings()
        {
            FileName = "tracker";
            ParseSupportedGameVersions();
            Load();
            TrySetGameVersion(GameVersion);
        }

        private void ParseSupportedGameVersions()
        {
            //get list of supported versions from assets folder
            SupportedVersions = new HashSet<string>();
            try
            {
                var directory = new DirectoryInfo(Paths.DIR_GAME_VERSIONS);
                if (directory.Exists)
                    foreach (var versionFolder in directory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                        SupportedVersions.Add(versionFolder.Name);
            }
            catch { Main.ForceQuit(); }
        }

        public override void ResetToDefaults()
        {
            Entries = new Dictionary<string, object>();
            TrySetGameVersion("1.12");
            Set(REFRESH_INTERVAL, 2000);
            Set(USE_DEFAULT_PATH, true);
            Set(AUTO_GAME_VERSION, true);
            Set(CUSTOM_FOLDER, string.Empty);
            TrySetGameVersion("1.16");
        }
    }
}

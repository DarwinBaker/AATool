using System;
using System.Collections.Generic;
using System.IO;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class TrackerSettings : SettingsGroup
    {
        public static TrackerSettings Instance = new TrackerSettings();

        public static readonly HashSet<string> SupportedVersions = new HashSet<string>() 
        { "1.16.2+", "1.16", "1.15", "1.14", "1.13", "1.12" };

        public const string GAME_VERSION       = "game_version";
        public const string REFRESH_INTERVAL   = "refresh_interval_ms";
        public const string USE_DEFAULT_PATH   = "use_default_saves_folder";
        public const string AUTO_GAME_VERSION  = "auto_game_version";
        public const string CUSTOM_FOLDER      = "custom_saves_folder";
        public const string SUPPORTED_VERSIONS = "supported_versions";

        public string GameVersion              { get => Get<string>(GAME_VERSION);      private set => GameVersion = value; }
        public int RefreshInterval             { get => Get<int>(REFRESH_INTERVAL);     set => Set(REFRESH_INTERVAL, value); }
        public bool UseDefaultPath             { get => Get<bool>(USE_DEFAULT_PATH);    set => Set(USE_DEFAULT_PATH, value); }
        public bool AutoDetectVersion          { get => Get<bool>(AUTO_GAME_VERSION);       set => Set(AUTO_GAME_VERSION, value); }
        public string CustomPath               { get => Get<string>(CUSTOM_FOLDER);     set => Set(CUSTOM_FOLDER, value); }

        public static string DefaultSavesFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves");
        public string SavesFolder               => UseDefaultPath ? DefaultSavesFolder : CustomPath;

        public void TrySetGameVersion(string version)
        {
            if (SupportedVersions.Contains(version))
                Set(GAME_VERSION, version);
        }

        private TrackerSettings()
        {
            FileName = "tracker";
            Load();
        }

        public override void ResetToDefaults()
        {
            Entries = new Dictionary<string, object>();
            Set(GAME_VERSION, "1.16");
            Set(REFRESH_INTERVAL, 1000);
            Set(USE_DEFAULT_PATH, true);
            Set(AUTO_GAME_VERSION, true);
            Set(CUSTOM_FOLDER, "");
            base.ResetToDefaults();
        }
    }
}

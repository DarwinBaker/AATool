using System;
using System.Collections.Generic;
using System.IO;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class TrackerSettings : SettingsGroup
    {
        public static TrackerSettings Instance = new TrackerSettings();

        private const string REFRESH_INTERVAL = "refresh_interval_ms";
        private const string USE_DEFAULT = "use_default_saves_folder";
        private const string CUSTOM_FOLDER = "custom_saves_folder";

        public int RefreshInterval { get => (int)Entries[REFRESH_INTERVAL]; set => Entries[REFRESH_INTERVAL] = value; }
        public bool UseDefaultPath { get => (bool)Entries[USE_DEFAULT];     set => Entries[USE_DEFAULT] = value; }
        public string CustomPath   { get => (string)Entries[CUSTOM_FOLDER]; set => Entries[CUSTOM_FOLDER] = value; }

        public static string DefaultSavesFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves");
        public string SavesFolder => UseDefaultPath ? DefaultSavesFolder : CustomPath;

        private TrackerSettings()
        {
            FileName = "tracker";
            Load();
        }

        public override void ResetToDefaults()
        {
            Entries = new Dictionary<string, object>()
            {
                { REFRESH_INTERVAL,  1000 },
                { USE_DEFAULT,       true },
                { CUSTOM_FOLDER,     "" },
            };
        }
    }
}

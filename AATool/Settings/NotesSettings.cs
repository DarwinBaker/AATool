using System.Collections.Generic;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class NotesSettings : SettingsGroup
    {
        public static NotesSettings Instance = new NotesSettings();

        public const string ENABLED       = "enabled";
        public const string ALWAYS_ON_TOP = "always_on_top";

        public bool Enabled     { get => Get<bool>(ENABLED);        set => Set(ENABLED, value); }
        public bool AlwaysOnTop { get => Get<bool>(ALWAYS_ON_TOP);  set => Set(ALWAYS_ON_TOP, value); }

        private NotesSettings()
        {
            FileName = "notes";
            Load();
        }

        public override void ResetToDefaults()
        {
            Entries = new Dictionary<string, object>();
            Set(ENABLED, false);
            Set(ALWAYS_ON_TOP, true);
        }
    }
}

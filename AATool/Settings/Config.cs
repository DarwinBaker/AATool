using AATool.UI;

namespace AATool.Settings
{
    public static class Config
    {
        public static TrackerSettings Tracker => TrackerSettings.Instance;
        public static MainSettings Main       => MainSettings.Instance;
        public static OverlaySettings Overlay => OverlaySettings.Instance;
        public static NotesSettings Notes     => NotesSettings.Instance;
        public static NetworkSettings Network => NetworkSettings.Instance;

        public static bool PostExplorationUpdate => TrackerSettings.PostExplorationUpdate;
        public static bool PostWorldOfColorUpdate => TrackerSettings.PostWorldOfColorUpdate;

        public static void ResetToDefaults()
        {
            Main.ResetToDefaults();
            Main.Save();
            Tracker.ResetToDefaults();
            Tracker.Save();
            Overlay.ResetToDefaults();
            Overlay.Save();
            Notes.ResetToDefaults();
            Notes.Save();
            Network.ResetToDefaults();
            Network.Save();
        }

        public static void ClearFlags()
        {
            Main.ClearFlags();
            Tracker.ClearFlags();
            Overlay.ClearFlags();
            Notes.ClearFlags();
            Network.ClearFlags();
        }
    }
}

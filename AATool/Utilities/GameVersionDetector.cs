using AATool.Settings;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AATool.Utilities
{
    public static class GameVersionDetector
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static void Update()
        {
            if (!Config.Tracker.AutoDetectVersion)
                return;

            //read active window title
            var builder = new StringBuilder(256);
            if (GetWindowText(GetForegroundWindow(), builder, 256) is 0)
                return;
            string title = builder.ToString().ToLower();
            if (title.Contains("minecraft"))
            {
                //title contains "minecraft"; parse version number
                string[] words = title.Split(' ');
                if (words.Length > 0)
                    TrySetVersion(words.Last());
            }
        }

        private static void TrySetVersion(string version)
        {
            //select appropriate tracker version
            if (version is "1.16.0" or "1.16.1")
                Config.Tracker.TrySetGameVersion("1.16");
            else if (version.StartsWith("1.16"))
                Config.Tracker.TrySetGameVersion("1.16.2+");
            else
                Config.Tracker.TrySetGameVersion(version.Substring(0, 4));
        }
    }
}

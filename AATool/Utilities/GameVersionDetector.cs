using AATool.Settings;
using System;
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
            //skip if disabled
            if (!Config.Tracker.AutoDetectVersion)
                return;

            //attempt to read active window title
            const int TitleLength = 256;
            var builder = new StringBuilder(TitleLength);
            if (GetWindowText(GetForegroundWindow(), builder, TitleLength) is 0)
                return;

            //attempt to parse second word in title as version
            string[] title = builder.ToString().Split(' ');
            if (title.Length > 1 && title[0].StartsWith("Minecraft"))
                Config.Tracker.TrySetGameVersion(title[1]);
        }
    }
}

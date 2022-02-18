using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AATool.Configuration;

namespace AATool.Utilities
{
    public static class ActiveInstance
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        const string InstanceNumberFileName = "instanceNumber.txt";
        const string GameDirFlag = "--gameDir ";
        const string NativesFlag = "-Djava.library.path=";
        const string NativesSlug = "natives";

        public static string SavesPath { get; private set; } = string.Empty;
        public static int Number { get; private set; } = -1;

        public static bool HasNumber => Number > 0;
        public static bool Watching => Config.Tracking.WatchActiveInstance;

        private static readonly Timer RefreshCooldown = new (1);

        private static int LastActiveInstance;

        public static void Update(Time time)
        {
            RefreshCooldown.Update(time);
            if (Watching && RefreshCooldown.IsExpired)
            {
                RefreshCooldown.Reset();

                //exit if minecraft is not active process or instance unchanged
                if (!TryGetActive(out Process instance) || instance.Id == LastActiveInstance)
                    return;

                //update saves folder
                string args = instance.CommandLine();
                SavesPath = TryParseDotMinecraft(args, out string dotMinecraft)
                    ? Path.Combine(dotMinecraft, "saves")
                    : string.Empty;

                //update instance properties
                UpdateInstanceNumber(dotMinecraft);
                UpdateGameVersion(instance);

                //prepare for next check
                LastActiveInstance = instance.Id;
            }
        }

        private static string CommandLine(this Process process)
        {
            string query = $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}";
            using (var searcher = new ManagementObjectSearcher(query))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>()
                    .SingleOrDefault()?["CommandLine"]?.ToString();
            }
        }

        private static bool TryGetActive(out Process instance)
        {
            instance = null;
            try
            {
                IntPtr hWnd = GetForegroundWindow();
                GetWindowThreadProcessId(hWnd, out uint processId);
                var active = Process.GetProcessById((int)processId);

                //verify that process is an instance of minecraft 
                if (active.ProcessName is "javaw" && active.MainWindowTitle.StartsWith("Minecraft"))
                    instance = active;
            }
            catch
            {
                //couldn't get active instance
            }
            return instance is not null;
        }

        private static bool TryParseDotMinecraft(string args, out string path)
        {
            path = string.Empty;
            if (string.IsNullOrEmpty(args))
                return false;

            try
            {
                //try parsing path
                //flag specifies ".minecraft" directory
                Match match = Regex.Match(args, @$"{GameDirFlag}(?:""(.+?)""|([^\s]+))");
                if (match.Success)
                {
                    path = match.Value.Substring(GameDirFlag.Length, match.Value.Length - GameDirFlag.Length);
                }
                else
                {
                    //try alternate method   
                    //flag specifies "natives" directory which is adjacent to ".minecraft"
                    match = Regex.Match(args, @$"{NativesFlag}(?:""(.+?)""|([^\s]+))");
                    if (match.Success)
                    {
                        path = match.Value.Substring(NativesFlag.Length, match.Value.Length - NativesFlag.Length);
                        //up a level
                        path = path.Substring(0, path.Length - NativesSlug.Length);
                        //back down
                        path = Path.Combine(path, ".minecraft");
                    }
                }
            }
            catch
            {
                //unable to parse .minecraft path
            }
            return !string.IsNullOrEmpty(path);
        }

        private static void UpdateInstanceNumber(string dotMinecraft)
        {
            Number = -1;
            if (string.IsNullOrEmpty(dotMinecraft))
                return;

            string numberPath = Path.Combine(dotMinecraft, InstanceNumberFileName);
            if (File.Exists(numberPath))
            {
                try
                {
                    Number = int.Parse(File.ReadAllText(numberPath));
                }
                catch
                {
                    //couldn't determine instance number
                }
            }
        }

        private static void UpdateGameVersion(Process instance)
        {
            if (Config.Tracking.AutoDetectVersion)
            {
                //get game version number from second word of title
                string[] title = instance.MainWindowTitle.Split(' ');
                if (title.Length > 1)
                    Tracker.TrySetVersion(title[1]);
            }
        }
    }
}

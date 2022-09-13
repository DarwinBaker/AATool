using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AATool.Configuration;
using AATool.Data.Categories;

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

        public static string SavesPath { get; private set; } = string.Empty;
        public static string LogFile { get; private set; } = string.Empty;
        public static int Number { get; private set; } = -1;
        public static int LastActiveId { get; private set; } = -1;

        public static bool HasNumber => Number > 0;
        public static bool Watching => Config.Tracking.WatchActiveInstance;

        private static readonly Timer RefreshCooldown = new (1);

        private static string LatestLogContents;
        private static string LatestGameVersion;
        private static DateTime LastLogWriteTimeUtc;
        private static int LogStart;

        public static void SetLogStart() => LogStart = LatestLogContents?.Length ?? 0;

        public static void Update(Time time)
        {
            RefreshCooldown.Update(time);
            if (Watching && RefreshCooldown.IsExpired)
            {
                RefreshCooldown.Reset();

                //exit if minecraft is not active process or instance unchanged
                if (!TryGetActive(out Process instance))
                    return;

                if (instance.Id != LastActiveId)
                {
                    Debug.BeginTiming("read_instance");

                    //update saves folder
                    string args = instance.CommandLine();
                    SavesPath = TryParseDotMinecraft(args, out DirectoryInfo dotMinecraft)
                        ? Path.Combine(dotMinecraft.FullName, "saves")
                        : string.Empty;

                    LogFile = dotMinecraft is not null
                        ? Path.Combine(dotMinecraft.FullName, "logs/latest.log")
                        : string.Empty;

                    //update instance properties
                    UpdateGameVersion(instance);
                    UpdateInstanceNumber(dotMinecraft?.FullName);

                    //prepare for next check
                    LastActiveId = instance.Id;

                    Debug.EndTiming("read_instance");
                }

                //update game version
                if (Config.Tracking.AutoDetectVersion && LatestGameVersion != Tracker.Category.CurrentVersion)
                    Tracker.TrySetVersion(LatestGameVersion);
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
                Debug.BeginTiming("get_active_instance");
                IntPtr hWnd = GetForegroundWindow();
                GetWindowThreadProcessId(hWnd, out uint processId);
                var active = Process.GetProcessById((int)processId);
                Debug.EndTiming("get_active_instance");

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

        private static bool TryParseDotMinecraft(string args, out DirectoryInfo folder)
        {
            folder = null;
            if (string.IsNullOrEmpty(args))
                return false;

            string path;
            try
            {
                if (args.Contains(GameDirFlag))
                {
                    //try parsing path
                    //flag specifies ".minecraft" directory
                    Match match = Regex.Match(args, @$"{GameDirFlag}(?:""(.+?)""|([^\s]+))");
                    path = args.Substring(match.Index + GameDirFlag.Length, match.Length - GameDirFlag.Length) + "\\";
                }
                else
                {
                    //try alternate method   
                    //flag specifies "natives" directory which is adjacent to ".minecraft"
                    Match match = Regex.Match(args, @$"(?:{NativesFlag}(.+?) )|(?:\""{NativesFlag}(.+?)\"")");
                    int length = match.Length;
                    int index = match.Index;
                    if (args[match.Index + NativesFlag.Length] is '=')
                    {
                        length -= 1;
                        index += 1;
                    }
                    path = args.Substring(index + NativesFlag.Length, length - NativesFlag.Length - 8) + ".minecraft\\";
                    path = path.Replace("/", "\\");
                }
                folder = new DirectoryInfo(path);
            }
            catch
            {
                //unable to parse .minecraft path
            }
            return folder is not null;
        }

        public static bool TryGetLog(out string latestLog)
        {
            latestLog = null;

            //aatool does not read the log file unless running all deaths
            if (Tracker.Category is not AllDeaths)
                return false;

            latestLog = latestLog?.Length > LogStart 
                ? LatestLogContents?.Substring(LogStart)
                : LatestLogContents;

            if (string.IsNullOrEmpty(LogFile))
                return false;

            try
            {
                //make sure file actually changed before bothering to read it
                DateTime latestLogWriteTimeUtc = File.GetLastWriteTimeUtc(LogFile);
                if (LastLogWriteTimeUtc != latestLogWriteTimeUtc || Config.Tracking.SourceChanged)
                {
                    LastLogWriteTimeUtc = latestLogWriteTimeUtc;

                    //read log contents and cache for later
                    using var stream = new FileStream(LogFile,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite | FileShare.Delete);
                    using (var reader = new StreamReader(stream))
                        LatestLogContents = latestLog = reader.ReadToEnd();

                    if (latestLog.Length > LogStart)
                        latestLog = latestLog.Substring(LogStart);

                    return true;
                }  
            }
            catch { }
            return false;
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
            //get game version number from second word of title
            string[] title = instance.MainWindowTitle.Split(' ');
            if (title.Length > 1)
                LatestGameVersion = title[1];
        }
    }
}

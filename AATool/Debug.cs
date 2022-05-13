using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace AATool
{
    public static class Debug
    {
        private static readonly Dictionary<string, Stopwatch> Watches = new ();

        public static bool EnableTiming { get; set; } = false;

        public static void BeginTiming(string name)
        {
            if (!EnableTiming)
                return;

            if (!Watches.TryGetValue(name, out Stopwatch watch))
                Watches[name] = watch = new Stopwatch();
            watch.Restart();
        }

        public static void EndTiming(string name)
        {
            if (!EnableTiming)
                return;

            if (Watches.TryGetValue(name, out Stopwatch watch))
                watch.Stop();
        }

        public static void SaveReport(Exception exception)
        {
            Directory.CreateDirectory(Paths.System.LogsFolder);
            using (StreamWriter stream = File.CreateText(Paths.System.CrashLogFile))
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                foreach (ManagementObject managementObject in searcher.Get())
                {
                    if (managementObject["Caption"] != null)
                        stream.WriteLine("OS: " + managementObject["Caption"].ToString());
                    if (managementObject["OSArchitecture"] != null)
                        stream.WriteLine("Architecture: " + managementObject["OSArchitecture"].ToString());
                    if (managementObject["CSDVersion"] != null)
                        stream.WriteLine("Service Pack: " + managementObject["CSDVersion"].ToString());
                }

                if (!Directory.Exists("assets"))
                    stream.WriteLine("\"assets\" Folder Missing!!!");

                stream.WriteLine("Exception: " + exception.Message);
                stream.Write(exception.StackTrace
                    .Replace("   at ", "\n    at ")
                    .Replace(") in ", ")\n        in file: ")
                    .Replace(":line ", "\n        on line: "));
                stream.Flush();
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using AATool.Utilities;

namespace AATool
{
    public static class Program
    {
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) =>
            SaveCrashReport(e.ExceptionObject as Exception);

        private static void GlobalThreadExceptionHandler(object sender, ThreadExceptionEventArgs e) =>
            SaveCrashReport(e.Exception);

        [STAThread]
        static void Main()
        {
            //add crash reporting events
            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            Application.ThreadException += GlobalThreadExceptionHandler;

            //start application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var main = new Main())
                main.Run();
        }

        private static void SaveCrashReport(Exception exception)
        {
            Directory.CreateDirectory(Paths.DIR_LOGS);
            using (StreamWriter stream = File.CreateText(Paths.CrashLogFile))
            {
                var mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                foreach (ManagementObject managementObject in mos.Get())
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

using System;
using System.IO;
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

            UpdateHelper.CheckAsync(true);

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

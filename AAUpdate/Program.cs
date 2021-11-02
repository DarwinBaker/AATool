using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AAUpdate
{
    static class Program
    {
        private const string FLAG_RETURN = "-r";
        private const string FLAG_OUTPUT = "-o";

        private static Updater Updater;

        private static bool IsOriginalExecutable =>
            Directory.GetCurrentDirectory() != Updater.TempUpdaterFolder;

        private static string ArgumentList =>
            $"{( Updater.ReturnWhenDone ? "-r" : "" )} -o \"{Directory.GetCurrentDirectory()}\"";

        [STAThread]
        static void Main(string[] args)
        {
            Updater = new();
            ReadCommandLineArgs(args);

            if (IsOriginalExecutable)
            {
                //this instance is in the install dir. re-run in temp location
                CloneTemporaryExecutable();
            }
            else
            {
                //show form
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FMain(Updater));

                //start AATool.exe if flag is set
                if (Updater.ReturnWhenDone)
                    StartMainProgram();
            }
        }

        private static void ReadCommandLineArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is FLAG_RETURN)
                {
                    //return to main program on exit
                    Updater.ReturnWhenDone = true;
                }
                else if (args[i] is FLAG_OUTPUT && i + 1 < args.Length)
                {
                    //output path to install updates to
                    Updater.SetDestination(Regex.Replace(args[i + 1], "^\"|\"$", ""));
                    i++;
                }
            }
        }

        private static void CloneTemporaryExecutable()
        {
            //clone and re-run update executable in temp dir (allows updater to update itself)
            Updater.CloneToTempFolder();
            var info = new ProcessStartInfo() {
                WorkingDirectory = Updater.TempUpdaterFolder,
                Arguments = ArgumentList,
                FileName = Updater.TempUpdaterExecutable
            };
            new Process() { StartInfo = info }.Start();
        }

        private static void StartMainProgram()
        {
            //re-launch aatool
            var info = new ProcessStartInfo() {
                FileName         = "AATool.exe",
                WorkingDirectory = Updater.Destination.FullName
            };
            new Process() { StartInfo = info }.Start();
        }
    }
}

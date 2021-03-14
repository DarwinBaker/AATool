using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AAUpdate
{
    static class Program
    {
        
        private static bool IsOriginalExecutable => Directory.GetCurrentDirectory() != updater.TempUpdaterFolder;
        
        private static Updater updater;

        [STAThread]
        static void Main(string[] args)
        {
            updater = new Updater();
            ReadCommandLineArgs(args);

            if (IsOriginalExecutable)
                CloneTemporaryExecutable();
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FMain(updater));

                if (updater.StartAAToolWhenDone)
                    StartMainProgram();
            }
        }

        private static void ReadCommandLineArgs(string[] args)
        {
            int index = 0;
            while (index < args.Length)
            {
                if (args[index] == "-r")
                {
                    //return to main program on exit
                    updater.StartAAToolWhenDone = true;
                    index++;
                }              
                else if (args[index] == "-o" && index + 1 < args.Length)
                {
                    //output path to install updates to
                    updater.SetDestination(Regex.Replace(args[index + 1], "^\"|\"$", ""));
                    index += 2;
                } 
                else
                    index++;
            }
        }

        private static void CloneTemporaryExecutable()
        {
            //copy this executable to temp folder and run from there (allows updater to update itself, as the original will not be in use)
            updater.CloneToTempFolder();

            //start temporary cloned update executable
            using (var process = new Process())
            {
                process.StartInfo.FileName = updater.TempUpdaterExecutable;
                process.StartInfo.WorkingDirectory = updater.TempUpdaterFolder;
                process.StartInfo.Arguments = (updater.StartAAToolWhenDone ? "-r" : "") + " -o \"" + Directory.GetCurrentDirectory() + "\"";
                process.Start();
            }
        }

        private static void StartMainProgram()
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "AATool.exe";
                process.StartInfo.WorkingDirectory = updater.Destination.FullName;
                process.Start();
            }
        }
    }
}

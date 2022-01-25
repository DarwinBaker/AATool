using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using AATool.Net.Requests;

namespace AATool.Utilities
{
    public static class UpdateHelper
    {
        public static bool UpdatesAreAvailable => Main.IsBeta 
            ? Main.Version <= UpdateRequest.LatestVersion 
            : Main.Version < UpdateRequest.LatestVersion;

        public static void RunAAUpdate(int exitCode)
        {
            //start update executable with "return to AATool after" flag
            Process.Start(Paths.System.UpdateExecutable, "-r");
            Environment.Exit(exitCode);
        }

        public static void CheckAsync(bool userTriggered = false)
        {
            Task.Run(async() => 
            {
                //get latest release info from github
                await new UpdateRequest().DownloadAsync();

                //defer update handling to main screen if this was the startup check
                if (userTriggered)
                    HandleUserCheck();
            });
        }

        private static void HandleUserCheck()
        {
            if (UpdatesAreAvailable)
            {
                //user manually requested the check, so go ahead and install updates
                RunAAUpdate(1);
            }
            else if (Main.IsBeta)
            {
                //inform user that betas don't auto-update until release and release isn't out yet
                MessageBox.Show($"You are currently running {Main.FullTitle}. Betas do not recieve automatic updates until official release.",
                    "Official Release Not Out Yet");
            }
            else
            {
                //inform user they're up-to-date
                MessageBox.Show($"You already have the lastest version ({Main.Version}) of CTM's AATool.", 
                    "No Updates Available");
            }
        }
    }
}

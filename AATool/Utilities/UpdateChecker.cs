using AATool.Winforms.Forms;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AATool.Utilities
{
    public static class UpdateHelper
    {
        public static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static double VersionStringToNumber(string versionString)
        {
            //turn version string into a double for comparisons (needed when comparing "major.minor.build" with "major.minor.build.revision")
            string[] split = versionString.Split('.');
            versionString  = string.Empty;
            for (int i = 0; i < split.Length; i++)
                versionString += (i == 1 ? "." : string.Empty) + split[i];

            return double.TryParse(versionString, out var versionDouble) ? versionDouble : 0;
        }

        private static async Task<string> DownloadLatestReleaseHTML()
        {
            //download html of latest release page
            using (var client = new WebClient())
                return await client.DownloadStringTaskAsync("https://github.com/DarwinBaker/AATool/releases/latest/");
        }

        private static string ParseLatestLink(string html)
        {
            //get download link for latest release zip
            int start = html.IndexOf("DarwinBaker/AATool/releases/download/");
            int end   = html.IndexOf(".zip");
            return html.Substring(start, end - start);
        }

        private static string ParseLatestVersion(string html)
        {
            //get version number of latest release
            string latestLink = ParseLatestLink(html);
            return latestLink.Substring(latestLink.LastIndexOf('_') + 1);
        }

        private static string ParsePatchNotes(string html)
        {
            //get patch notes of latest release
            string patchNotes = html.Substring(html.IndexOf("<div class=\"markdown-body\">"));
            return Regex.Replace(patchNotes.Substring(0, patchNotes.IndexOf("<details")), @"<[^>]*>", string.Empty);
        }

        public static async void TryCheckForUpdatesAsync(bool failSilently = true)
        {
            try
            {
                //get latest release info from github
                string html          = await DownloadLatestReleaseHTML();
                string latestVersion = ParseLatestVersion(html);
                string patchNotes    = ParsePatchNotes(html);

                //compare version numbers to determine if updates are available
                if (VersionStringToNumber(latestVersion) <= VersionStringToNumber(CurrentVersion))
                {
                    //aready have the latest version
                    if (!failSilently)
                        System.Windows.Forms.MessageBox.Show($"You already have the lastest version ({CurrentVersion}) of CTM's AATool.", "No Updates Available");
                    
                }
                else
                {
                    //updates are available; ask user if they'd like to download
                    using (var dialog = new FUpdate(latestVersion, patchNotes))
                        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                            RunUpdateAssistant(false);
                }
            }
            catch (Exception) { }
        }

        public static void RunUpdateAssistant(bool isError = false)
        {
            //start update executable with "return to AATool" flag
            using (var process = new Process())
            {
                process.StartInfo.FileName = "AAUpdate.exe";
                process.StartInfo.Arguments = "-r";
                process.Start();
                Environment.Exit(isError ? 1 : 0);
            }
        }
    }
}

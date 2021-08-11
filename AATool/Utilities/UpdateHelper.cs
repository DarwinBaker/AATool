using AATool.Settings;
using AATool.Winforms.Forms;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AATool.Utilities
{
    public static class UpdateHelper
    {
        public static string current => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static double ToNumber(string versionString)
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
            using WebClient client = new ();
            try
            {
                return await client.DownloadStringTaskAsync("https://github.com/DarwinBaker/AATool/releases/latest/");
            }
            catch (Exception)
            {
                return string.Empty;
            }
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

        public static void ShowFirstSetupMessage()
        {
            bool isDarkMode = true;
            (Color back, Color text, Color border) = MainSettings.Themes["Dark Mode"];
            isDarkMode &= Config.Main.BackColor   == back;
            isDarkMode &= Config.Main.TextColor   == text;
            isDarkMode &= Config.Main.BorderColor == border;
            if (isDarkMode)
                return;

            string title = $"Welcome to the AATool Co-op Update!";
            string body  = $"Dark mode is the new default theme! Would you like to give it a try?";
            DialogResult result = MessageBox.Show(body, title, MessageBoxButtons.YesNo);
            if (result is DialogResult.Yes)
            {
                Config.Main.BackColor   = back;
                Config.Main.TextColor   = text;
                Config.Main.BorderColor = border;
                Config.Main.Save();
            }
        }

        public static async void CheckAsync(bool failSilently = true)
        {
            try
            {
                //get latest release info from github
                string html = await DownloadLatestReleaseHTML();
                if (string.IsNullOrEmpty(html))
                    return;

                string latest = ParseLatestVersion(html);
                string patchNotes = ParsePatchNotes(html);

                //compare version numbers to determine if updates are available
                if (ToNumber(current) >= ToNumber(latest))
                {
                    if (Main.IsBeta && ToNumber(current) == ToNumber(latest))
                        ShowDialog(true, latest, patchNotes);
                    else if (!failSilently)
                        ShowDialog(false);
                }
                else if (!failSilently)
                {
                    ShowDialog(false);
                }
            }
            catch { }
        }

        private static void ShowDialog(bool updatesAvailable, string latest = null, string patchNotes = null)
        {
            if (updatesAvailable)
            {
                //ask user if they'd like to download now
                using var dialog = new FUpdate(latest, patchNotes);
                if (dialog.ShowDialog() is DialogResult.Yes)
                    RunUpdateAssistant(false);
            }
            else if (Main.IsBeta)
            {
                MessageBox.Show($"You are currently running {Main.FullTitle}. Betas " +
                    $"do not recieve automatic updates until official release.", "Official Release Not Out Yet");
            }
            else
            {
                MessageBox.Show($"You already have the lastest version ({current}) of CTM's AATool.", "No Updates Available");
            }
        }

        public static void RunUpdateAssistant(bool isError = false)
        {
            //start update executable with "return to AATool" flag
            using var process = new Process();
            process.StartInfo.FileName  = "AAUpdate.exe";
            process.StartInfo.Arguments = "-r";
            process.Start();
            Environment.Exit(isError ? 1 : 0);
        }
    }
}

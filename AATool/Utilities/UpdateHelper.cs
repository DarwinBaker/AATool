using AATool.Settings;
using AATool.Winforms.Forms;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        private static async Task<string> DownloadLatestReleaseHTML()
        {
            //download html of latest release page
            using WebClient client = new ();
            try
            {
                return await client.DownloadStringTaskAsync("https://github.com/DarwinBaker/AATool/releases/latest/");
            }
            catch
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

        private static Version ParseLatestVersion(string html)
        {
            //get version number of latest release
            string latest = ParseLatestLink(html);
            latest = latest.Substring(latest.LastIndexOf('_') + 1);
            Version.TryParse(latest, out Version version);
            return version;
        }

        private static string ParsePatchNotes(string html)
        {
            //get patch notes of latest release
            string patchNotes = html.Substring(html.IndexOf("<div class=\"markdown-body\">"));
            return Regex.Replace(patchNotes.Substring(0, patchNotes.IndexOf("<details")), @"<[^>]*>", string.Empty);
        }

        public static async void CheckAsync(bool failSilently = true)
        {
            try
            {
                //get latest release info from github
                string html = await DownloadLatestReleaseHTML();
                if (string.IsNullOrEmpty(html))
                    return;

                Version latest = ParseLatestVersion(html);
                string patchNotes = ParsePatchNotes(html);

                //compare version numbers to determine if updates are available
                if (Main.Version == latest)
                {
                    if (Main.IsBeta)
                    {
                        using var dialog = new FUpdate(latest, patchNotes);
                            if (dialog.ShowDialog() is DialogResult.Yes)
                                RunUpdateAssistant(false);
                    }
                    else if (!failSilently)
                    {
                        ShowDialog();
                    }
                }
                else if (Main.Version < latest)
                {
                    using var dialog = new FUpdate(latest, patchNotes);
                    if (dialog.ShowDialog() is DialogResult.Yes)
                        RunUpdateAssistant(false);
                }
                else if (!failSilently)
                {
                    ShowDialog();
                }
            }
            catch { }
        }

        private static void ShowDialog()
        {
            if (Main.IsBeta)
            {
                MessageBox.Show($"You are currently running {Main.FullTitle}. Betas " +
                    $"do not recieve automatic updates until official release.", "Official Release Not Out Yet");
            }
            else
            {
                MessageBox.Show($"You already have the lastest version ({Main.Version}) of CTM's AATool.", "No Updates Available");
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

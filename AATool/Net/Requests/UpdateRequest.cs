using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.Net.Requests
{
    public sealed class UpdateRequest : NetRequest
    {
        private const string PatchNotesUrl = "https://github.com/DarwinBaker/AATool/releases/latest/download/patch_notes.xml";
        private const string ThumbnailUrl = "https://github.com/DarwinBaker/AATool/releases/latest/download/thumbnail.png";

        public static List<(string text, string icon)> LatestUpgrades { get; private set; } = new ();
        public static List<(string text, string icon)> LatestFixes    { get; private set; } = new ();
        public static Version LatestVersion { get; private set; }
        public static Texture2D LatestThumb { get; private set; }
        public static string LatestTitle    { get; private set; }

        private static XmlDocument LatestPatch;

        public static bool Suppress { get; set; }
        public static bool UserInitiated { get; set; }

        public static bool IsDone => LatestVersion is not null;

        public UpdateRequest(bool isManual = false) : base(PatchNotesUrl)
        {
            Suppress = false;
            UserInitiated = isManual;
        }

        public static bool UpdatesAreAvailable()
        {
            if (LatestVersion is null)
                return false;

            return Main.IsBeta
                ? Main.Version <= LatestVersion
                : Main.Version < LatestVersion;
        }

        public override async Task<bool> DownloadAsync()
        {
            using var client = new HttpClient() { 
                Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutMs)
            };
            try
            {
                //get latest update information from github
                string latestXml = await client.GetStringAsync(PatchNotesUrl);

                using (Stream imageStream = await client.GetStreamAsync(ThumbnailUrl))
                    LatestThumb = Texture2D.FromStream(Main.GraphicsManager.GraphicsDevice, imageStream);

                return this.HandleResponse(latestXml);  
            }
            catch (Exception e)
            {
                //nothing to do if network error
                if (e is HttpRequestException)
                    return false;

                //nothing to do if canceled
                if (e is OperationCanceledException)
                    return false;

                //nothing to do if image preview is malformed
                if (e is InvalidOperationException)
                    return false;

                throw;
            }
        }

        private bool HandleResponse(string latestXml)
        {
            if (string.IsNullOrEmpty(latestXml))
                return false;

            try
            {
                LatestPatch = new XmlDocument();
                LatestPatch.LoadXml(latestXml);

                //get version
                LatestTitle = LatestPatch.DocumentElement.GetAttribute("title");
                string vnum = LatestPatch.DocumentElement.GetAttribute("version");

                //populate change lists
                LatestUpgrades.Clear();
                foreach (XmlNode upgrade in LatestPatch.DocumentElement.SelectSingleNode("upgrades").ChildNodes)
                    LatestUpgrades.Add((upgrade.InnerText, upgrade.Attributes["icon"]?.Value ?? "bullet_point"));
                LatestFixes.Clear();
                foreach (XmlNode fix in LatestPatch.DocumentElement.SelectSingleNode("fixes").ChildNodes)
                    LatestFixes.Add((fix.InnerText, fix.Attributes["icon"]?.Value ?? "bullet_fix"));

                if (Version.TryParse(vnum, out Version version))
                {
                    LatestVersion = version;
                    return true;
                }
            }
            catch (NullReferenceException)
            {
                //malformed response, nothing to do here
            }
            catch (XmlException)
            {
                //malformed response, nothing to do here
            }

            if (UserInitiated)
                MessageBox.Show("There was a problem checking for updates! Try again later, and make sure your firewall isn't blocking AATool.", "Couldn't Get Latest Version", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            return false;
        }
    }
}

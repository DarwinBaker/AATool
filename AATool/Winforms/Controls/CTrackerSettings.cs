using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Net;
using AATool.Saves;
using AATool.Winforms.Forms;

namespace AATool.Winforms.Controls
{
    public partial class CTrackerSettings : UserControl
    {
        private bool loaded;

        private static Image SoloAvatar;
        private CancellationTokenSource cancelSource;

        public CTrackerSettings()
        {
            this.InitializeComponent();
        }

        public void LoadSettings()
        {
            this.loaded = false;
            this.autoVersion.Checked = Config.Tracking.AutoDetectVersion;
            this.UpdateCategoryControls();

            this.worldRemote.Checked = Config.Tracking.UseSftp;
            this.worldLocal.Checked = !Config.Tracking.UseSftp;

            this.filterCombined.Checked = Config.Tracking.Filter == ProgressFilter.Combined;
            this.filterSolo.Checked = Config.Tracking.Filter == ProgressFilter.Solo;
            this.filterSoloName.Text = Config.Tracking.SoloFilterName;

            this.trackActiveInstance.Checked = Config.Tracking.Source == TrackerSource.ActiveInstance;
            this.trackCustomSavesFolder.Checked = Config.Tracking.Source == TrackerSource.CustomSavesPath;
            this.TrackSpecificWorld.Checked = Config.Tracking.Source == TrackerSource.SpecificWorld;

            this.customSavesPath.Text = Config.Tracking.CustomSavesPath;
            this.customWorldPath.Text = Config.Tracking.CustomWorldPath;

            this.gameVersion.Text = Config.Tracking.GameVersion;

            this.enableOpenTracker.Checked = Config.Tracking.BroadcastProgress;

            this.sftpHost.Text = Config.Sftp.Host;
            this.sftpPort.Text = Config.Sftp.Port.Value.ToString();
            this.sftpUser.Text = Config.Sftp.Username;
            this.sftpPass.Text = Config.Sftp.Password;
            this.sftpRoot.Text = Config.Sftp.ServerRoot;
            this.sftpAutoSaveMinutes.Value = Config.Sftp.AutoSaveMinutes;
            this.sftpType.Text = Config.Sftp.Linux ? "Linux" : "Windows";

            this.UpdateSaveGroupPanel();
            this.UpdateFilterPanel();

            if (SoloAvatar is null)
            {
                this.cancelSource?.Cancel();
                this.cancelSource = new CancellationTokenSource();
                this.TryUpdateSoloFilterAsync(this.cancelSource.Token);
            }
            else
            {
                this.soloAvatar.Image = SoloAvatar;
            }
            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (this.loaded)
            {
                if (this.trackActiveInstance.Checked)
                    Config.Tracking.Source.Set(TrackerSource.ActiveInstance);
                else if (this.trackCustomSavesFolder.Checked)
                    Config.Tracking.Source.Set(TrackerSource.CustomSavesPath);
                else if (this.TrackSpecificWorld.Checked)
                    Config.Tracking.Source.Set(TrackerSource.SpecificWorld);

                Config.Tracking.CustomSavesPath.Set(this.customSavesPath.Text);
                Config.Tracking.CustomWorldPath.Set(this.customWorldPath.Text);

                if (this.filterCombined.Checked)
                    Config.Tracking.Filter.Set(ProgressFilter.Combined);
                else if (this.filterSolo.Checked)
                    Config.Tracking.Filter.Set(ProgressFilter.Solo);

                Config.Tracking.SoloFilterName.Set(this.filterSoloName.Text);

                TrackerSource source = this.trackActiveInstance.Checked
                    ? TrackerSource.ActiveInstance
                    : this.trackCustomSavesFolder.Checked
                        ? TrackerSource.CustomSavesPath
                        : TrackerSource.SpecificWorld;
                Config.Tracking.Source.Set(source);

                Config.Tracking.UseSftp.Set(this.worldRemote.Checked);
                Config.Tracking.AutoDetectVersion.Set(this.autoVersion.Checked);
                Config.Tracking.BroadcastProgress.Set(this.enableOpenTracker.Checked);
                Config.Tracking.TrySave();

                if (int.TryParse(this.sftpPort.Text, out int port))
                    Config.Sftp.Port.Set(port);

                Config.Sftp.Host.Set(this.sftpHost.Text);
                Config.Sftp.Username.Set(this.sftpUser.Text);
                Config.Sftp.Password.Set(this.sftpPass.Text);
                Config.Sftp.ServerRoot.Set(this.sftpRoot.Text);
                Config.Sftp.AutoSaveMinutes.Set((int)Math.Max(1, this.sftpAutoSaveMinutes.Value));
                Config.Sftp.Linux.Set(this.sftpType.Text == "Linux");
                Config.Sftp.TrySave();
            }
        }

        public void InvalidateSettings()
        {
            this.LoadSettings();
        }

        private void UpdateCategoryControls()
        {
            this.category.Text    = Tracker.Category.Name;
            this.category.Enabled = !Peer.IsClient;
            if (Config.Tracking.GameCategory.Changed || !this.loaded)
            {
                this.gameVersion.Items.Clear();
                foreach (string version in Tracker.Category.GetSupportedVersions())
                    this.gameVersion.Items.Add(version);
            }
            if (Config.Tracking.GameVersion.Changed || !this.loaded)
            {
                this.gameVersion.Text = Tracker.Category.CurrentVersion;
            }
            this.gameVersion.Enabled = !this.autoVersion.Checked && !Peer.IsClient && this.gameVersion.Items.Count > 1;
        }


        private void UpdateSaveGroupPanel()
        {
            if (this.worldLocal.Checked)
            {
                this.localGroup.Visible = true;
                this.remoteGroup.Visible = false;
            }
            else
            {
                this.localGroup.Visible = false;
                this.remoteGroup.Visible = true;
            }
        }

        private void UpdateFilterPanel()
        {
            this.filterSoloName.Enabled = this.filterSolo.Checked;
        }

        private void TogglePassword()
        {
            bool hide = true;
            if (this.sftpPass.UseSystemPasswordChar)
            {
                //show confirmation dialog
                string message = "Be careful about showing SFTP login credentials on stream! ♥\nAre you sure you want to unmask the username and password fields?";
                string title   = "SFTP Credentials Reveal Confirmation";
                DialogResult result = MessageBox.Show(this, message, title,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                hide = result is not DialogResult.Yes;
            }
            this.sftpPass.UseSystemPasswordChar = hide;
            this.sftpUser.UseSystemPasswordChar = hide;
            this.toggleCredentials.Text = hide
                ? "Show Login"
                : "Hide Login";
        }

        private void OnClicked(object sender, EventArgs e)
        {
            if (sender == this.browseSaves)
            {
                using var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                    this.customSavesPath.Text = dialog.SelectedPath;
            }
            else if (sender == this.browseWorld)
            {
                using var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                    this.customWorldPath.Text = dialog.SelectedPath;
            }
            else if (sender == this.sftpValidate)
            {
                MinecraftServer.Sync();
            }
            else if (sender == this.toggleCredentials)
            {
                this.TogglePassword();
            }
            else if (sender == this.configureOpenTracker)
            {
                using (var dialog = new FOpenTrackerSetup())
                    dialog.ShowDialog();
            }
            this.SaveSettings();
        }

        private void OnCheckChanged(object sender, EventArgs e)
        {
            if (!this.loaded)
                return;

            if (sender == this.worldLocal || sender == this.worldRemote)
            {
                this.UpdateSaveGroupPanel();
            }
            else if (sender == this.filterCombined || sender == this.filterSolo)
            {
                this.UpdateFilterPanel();
            }
            else if (sender == this.autoVersion)
            {
                this.UpdateCategoryControls();
            }
            this.SaveSettings();
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            if (!this.loaded)
                return;

            if (sender == this.category)
            {
                Tracker.TrySetCategory(this.category.Text);
            }
            else if (sender == this.gameVersion)
            {
                Tracker.TrySetVersion(this.gameVersion.Text);
            }
            this.SaveSettings();
        }

        private void OnTextChanged(object sender, EventArgs e) 
        {
            if (sender == this.filterSoloName)
            {
                //cancel old requests and start a new one
                this.cancelSource?.Cancel();
                this.cancelSource = new CancellationTokenSource();

                //cooldown timer to prevent spamming requests for every letter of someone's name
                this.keyboardTimer.Start();
            }
            this.SaveSettings();
        }

        private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender == this.sftpCompatibility)
            {
                string title = "SFTP Compatibility Information";
                string body = "Remote tracking over SFTP has only been officially tested on DedicatedMC, " +
                    "although other hosts should work as well.";
                MessageBox.Show(this, body, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            //enough time has passed since last character was typed
            this.keyboardTimer.Stop();
            this.TryUpdateSoloFilterAsync(this.cancelSource.Token);
        }

        private async void TryUpdateSoloFilterAsync(CancellationToken? cancelToken = null)
        {
            if (!Uuid.TryParse(this.filterSoloName.Text, out Uuid id))
            {
                if (Player.ValidateName(this.filterSoloName.Text))
                    id = await Player.FetchUuidAsync(this.filterSoloName.Text);
            }

            if (id == Uuid.Empty)
            {
                this.soloAvatar.Image = null;
                return;
            }

            if (this.soloAvatar.Image is null || (this.soloAvatar.Image.Tag is Uuid current && id != current))
            {
                try
                {
                    //asynchronously pull player avatar from the internet
                    string url = Paths.Web.GetAvatarUrl(id.String, 16);
                    using HttpClient http = new ();
                    using HttpResponseMessage responce = await http.GetAsync(new Uri(url), cancelToken ?? CancellationToken.None);
                    using Stream stream = await responce.Content.ReadAsStreamAsync();
                    this.soloAvatar.Image = new Bitmap(stream) {
                        Tag = id
                    };
                    SoloAvatar = this.soloAvatar.Image;
                }
                catch { }
            }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            this.SaveSettings();
        }
    }
}

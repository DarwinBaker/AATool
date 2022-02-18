using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Net;
using AATool.Saves;
using AATool.Utilities;
using AATool.Winforms.Forms;
using Microsoft.Xna.Framework;

namespace AATool.Winforms.Controls
{
    public partial class CTrackerSettings : UserControl
    {
        private bool loaded;

        public CTrackerSettings()
        {
            this.InitializeComponent();
            this.autoVersion.Checked = Config.Tracking.AutoDetectVersion;
        }

        public void LoadSettings()
        {
            this.loaded = false;
            this.UpdateCategoryControls();

            this.worldRemote.Checked  = Config.Tracking.UseSftp;
            this.worldLocal.Checked   = !Config.Tracking.UseSftp;

            this.trackActiveInstance.Checked = Tracker.Source is TrackerSource.ActiveInstance;
            this.trackDefaultSaves.Checked = Tracker.Source is TrackerSource.DefaultAppData;
            this.trackCustomSavesFolder.Checked = Tracker.Source is TrackerSource.CustomSavesPath;
            this.TrackSpecificWorld.Checked = Tracker.Source is TrackerSource.SpecificWorld;

            this.customSavesPath.Text = Config.Tracking.CustomSavesPath;
            this.customWorldPath.Text = Config.Tracking.CustomWorldPath;

            this.autoVersion.Checked = Config.Tracking.AutoDetectVersion;
            this.gameVersion.Text = Config.Tracking.GameVersion;

            this.enableOpenTracker.Checked = Config.Tracking.BroadcastProgress;

            this.sftpHost.Text = Config.Sftp.Host;
            this.sftpPort.Text = Config.Sftp.Port.Value.ToString();
            this.sftpUser.Text = Config.Sftp.Username;
            this.sftpPass.Text = Config.Sftp.Password;
            this.sftpRoot.Text = Config.Sftp.ServerRoot;

            this.UpdateSaveGroupPanel();

            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (this.loaded)
            {
                if (this.trackActiveInstance.Checked)
                    Config.Tracking.Source.Set(TrackerSource.ActiveInstance);
                else if (this.trackDefaultSaves.Checked)
                    Config.Tracking.Source.Set(TrackerSource.DefaultAppData);
                else if (this.trackCustomSavesFolder.Checked)
                    Config.Tracking.Source.Set(TrackerSource.CustomSavesPath);
                else if (this.TrackSpecificWorld.Checked)
                    Config.Tracking.Source.Set(TrackerSource.SpecificWorld);

                Config.Tracking.CustomSavesPath.Set(this.customSavesPath.Text);
                Config.Tracking.CustomWorldPath.Set(this.customWorldPath.Text);

                Config.Tracking.Source.Set(this.trackDefaultSaves.Checked);
                Config.Tracking.UseSftp.Set(this.worldRemote.Checked);
                Config.Tracking.AutoDetectVersion.Set(this.autoVersion.Checked);
                Config.Tracking.BroadcastProgress.Set(this.enableOpenTracker.Checked);
                Config.Tracking.Save();

                if (int.TryParse(this.sftpPort.Text, out int port))
                    Config.Sftp.Port.Set(port);

                Config.Sftp.Host.Set(this.sftpHost.Text);
                Config.Sftp.Username.Set(this.sftpUser.Text);
                Config.Sftp.Password.Set(this.sftpPass.Text);
                Config.Sftp.ServerRoot.Set(this.sftpRoot.Text);
                Config.Sftp.Save();
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
                SftpSave.Sync();
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
    }
}

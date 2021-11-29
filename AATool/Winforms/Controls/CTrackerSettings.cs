using AATool.Net;
using AATool.Saves;
using AATool.Settings;
using AATool.UI;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AATool.Winforms.Controls
{
    public partial class CTrackerSettings : UserControl
    {
        private bool loaded;

        public CTrackerSettings()
        {
            this.InitializeComponent();
            this.category.Text = "All Advancements";
        }

        public void LoadSettings()
        {
            this.loaded = false;
            this.gameVersion.Items.Clear();
            foreach (Version version in TrackerSettings.SupportedVersions.Reverse())
                this.gameVersion.Items.Add(version.ToString());
            this.gameVersion.Text       = Config.Tracker.GameVersion;
            this.gameVersion.Enabled    = !Peer.IsClient;

            this.worldRemote.Checked    = Config.Tracker.UseRemoteWorld;
            this.worldLocal.Checked     = !Config.Tracker.UseRemoteWorld;

            this.savesDefault.Checked   = Config.Tracker.UseDefaultPath;
            this.savesCustom.Checked    = !Config.Tracker.UseDefaultPath;
            this.customSavePath.Text    = Config.Tracker.CustomPath;
            this.gameVersion.Text       = Config.Tracker.GameVersion;
            this.autoVersion.Checked    = Config.Tracker.AutoDetectVersion;
            this.sftpHost.Text          = Config.Tracker.SftpHost;
            this.sftpPort.Text          = Config.Tracker.SftpPort.ToString();
            this.sftpUser.Text          = Config.Tracker.SftpUser;
            this.sftpPass.Text          = Config.Tracker.SftpPass;

            this.UpdateSavePanels();

            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (!this.loaded)
                return;

            Config.Tracker.UseDefaultPath    = this.savesDefault.Checked;
            Config.Tracker.UseRemoteWorld    = this.worldRemote.Checked;
            Config.Tracker.CustomPath        = this.customSavePath.Text;
            Config.Tracker.AutoDetectVersion = this.autoVersion.Checked;
            Config.Tracker.TrySetGameVersion(this.gameVersion.Text);
            
            if (int.TryParse(this.sftpPort.Text, out int port))
                Config.Tracker.SftpPort = port;

            Config.Tracker.SftpHost = this.sftpHost.Text;
            Config.Tracker.SftpUser = this.sftpUser.Text;
            Config.Tracker.SftpPass = this.sftpPass.Text;
            Config.Tracker.Save();
        }

        public void UpdateGameVersion()
        {
            this.gameVersion.Text = Config.Tracker.GameVersion;
            this.gameVersion.Enabled = !Peer.IsClient;
        }

        private void UpdateSavePanels()
        {
            if (this.worldLocal.Checked)
            {
                this.localGroup.Enabled = true;
                this.remoteGroup.Enabled = false;
            }
            else
            {
                this.localGroup.Enabled = false;
                this.remoteGroup.Enabled = true;
            }
        }

        private void TogglePassword()
        {
            bool hide = true;
            if (this.sftpPass.UseSystemPasswordChar)
            {
                //show confirmation dialog
                string message = "Be careful about showing SFTP login credentials on stream! ♥\nAre you sure you want to unmask the username and password fields?";
                string title   = "SFTP Credentials Reaveal Confirmation";
                DialogResult result = MessageBox.Show(this, message, title,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                hide = result is not DialogResult.Yes;
            }
            this.sftpPass.UseSystemPasswordChar = hide;
            this.sftpUser.UseSystemPasswordChar = hide;
            this.toggleCredentials.Text = hide 
                ? "Show Login Credentials" 
                : "Hide Login Credentials";
        }

        private void OnClicked(object sender, EventArgs e)
        {
            if (sender == this.browse)
            {
                using var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                    this.customSavePath.Text = dialog.SelectedPath;
            }
            else if (sender == this.sftpValidate)
            {
                SftpSave.Sync();
            }
            else if (sender == this.toggleCredentials)
            {
                this.TogglePassword();
            }
            this.SaveSettings();
        }

        private void OnCheckChanged(object sender, EventArgs e)
        {
            if (sender == this.savesDefault || sender == this.savesCustom)
            {
                this.customSavePath.Enabled = !this.savesDefault.Checked;
                this.browse.Enabled = !this.savesDefault.Checked;
            }
            else if (sender == this.worldLocal || sender == this.worldRemote)
            {
                this.UpdateSavePanels();
            }
            else if (sender == this.autoVersion)
            {
                this.gameVersion.Enabled = !this.autoVersion.Checked && !Peer.IsClient;
            }
            this.SaveSettings();
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            if (sender == this.category)
            if (sender == this.gameVersion)
                Config.Tracker.TrySetGameVersion(this.gameVersion.Text);
            this.SaveSettings();
        }

        private void OnTextChanged(object sender, EventArgs e) => this.SaveSettings();

        private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender == this.sftpCompatibility)
            {
                string title = "SFTP Compatibility Information";
                string body = "Remote tracking over SFTP has only been officially tested on DedicatedMC, " +
                    "although other hosts should work as long as they keep \"server.properties\" in the root directory.";
                MessageBox.Show(this, body, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Net;
using AATool.Saves;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.Winforms.Controls
{
    public partial class CTrackerSettings : UserControl
    {
        private bool loaded;

        public CTrackerSettings()
        {
            this.InitializeComponent();
        }

        public void LoadSettings()
        {
            this.loaded = false;
            this.UpdateCategoryControls();

            this.worldRemote.Checked  = Config.Tracking.UseSftp;
            this.worldLocal.Checked   = !Config.Tracking.UseSftp;

            this.savesDefault.Checked = Config.Tracking.UseDefaultPath;
            this.savesCustom.Checked = !Config.Tracking.UseDefaultPath;
            this.customSavePath.Text = Config.Tracking.CustomSavePath;
            this.autoVersion.Checked = Config.Tracking.AutoDetectVersion;
            this.gameVersion.Text = Config.Tracking.GameVersion;

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
                Config.Tracking.UseDefaultPath.Set(this.savesDefault.Checked);
                Config.Tracking.UseSftp.Set(this.worldRemote.Checked);
                Config.Tracking.CustomSavePath.Set(this.customSavePath.Text);
                Config.Tracking.AutoDetectVersion.Set(this.autoVersion.Checked);
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
            if (Configuration.Config.Tracking.GameCategory.Changed || !this.loaded)
            {
                this.gameVersion.Items.Clear();
                foreach (string version in Tracker.Category.GetSupportedVersions())
                    this.gameVersion.Items.Add(version);
            }
            if (Configuration.Config.Tracking.GameVersion.Changed || !this.loaded)
            {
                this.gameVersion.Text = Tracker.Category.CurrentVersion;
            }
            this.gameVersion.Enabled = !this.autoVersion.Checked && !Peer.IsClient && this.gameVersion.Items.Count > 1;
        }


        private void UpdateSaveGroupPanel()
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
            if (!this.loaded)
                return;

            if (sender == this.savesDefault || sender == this.savesCustom)
            {
                this.customSavePath.Enabled = !this.savesDefault.Checked;
            }
            else if (sender == this.worldLocal || sender == this.worldRemote)
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
            else if (sender == this.serverPathHelp)
            {
                string title = "SFTP Server Path Information";
                string body = "Allows you to specify a custom path to your Minecraft server if it's not kept in the root directory. (for example, on a custom linux server).\n\n" +
                    "This should almost always be left blank, as most dedicated hosts do indeed keep the Minecraft server in the root directory.\n\n" +
                    "If AATool gives an error about paths not found, double check your server's directory structure and adjust this if necessary.";
                MessageBox.Show(this, body, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

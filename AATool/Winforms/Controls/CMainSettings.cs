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
    public partial class CMainSettings : UserControl
    {
        private bool loaded;

        public CMainSettings()
        {
            this.InitializeComponent();
            this.localGroup.Location  = new System.Drawing.Point(117, 3);
            this.remoteGroup.Location = new System.Drawing.Point(117, 3);
        }

        public void LoadSettings()
        {
            this.loaded = false;
            this.gameVersion.Items.Clear();
            foreach (string version in TrackerSettings.SupportedVersions.Reverse())
                this.gameVersion.Items.Add(version);
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


            this.viewMode.Text              = Config.Main.CompactMode ? "Compact" : "Relaxed";
            this.showBasic.Checked          = Config.Main.ShowBasic;
            this.completionGlow.Checked     = Config.Main.CompletionGlow;
            this.hideCompleted.Checked      = Config.Main.HideCompleted;
            this.layoutDebug.Checked        = Config.Main.LayoutDebug;
            this.refreshIcon.SelectedIndex  = Config.Main.RefreshIcon is "xp_orb" ? 0 : 1;
            this.backColor.BackColor        = ColorHelper.ToDrawing(Config.Main.BackColor);
            this.textColor.BackColor        = ColorHelper.ToDrawing(Config.Main.TextColor);
            this.borderColor.BackColor      = ColorHelper.ToDrawing(Config.Main.BorderColor);

            this.notesEnabled.Checked = Config.Notes.Enabled;

            //colors
            this.theme.Items.Clear();
            foreach (KeyValuePair<string, (Color, Color, Color)> theme in MainSettings.Themes)
            {
                this.theme.Items.Add(theme.Key);
                if (Config.Main.BackColor == theme.Value.Item1 && Config.Main.TextColor == theme.Value.Item2 && Config.Main.BorderColor == theme.Value.Item3)
                    this.theme.Text = theme.Key;
            }
            this.theme.Items.Add("Pride Mode");
            this.theme.Items.Add("Custom");
            if (Config.Main.RainbowMode)
                this.theme.Text = "Pride Mode";
            else if (string.IsNullOrEmpty(this.theme.Text))
                this.theme.Text = "Custom";

            this.UpdateSaveGroupPanel();

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

            Config.Main.CompactMode         = this.viewMode.Text.ToLower() is "compact";
            Config.Main.ShowBasic           = this.showBasic.Checked;
            Config.Main.CompletionGlow      = this.completionGlow.Checked;
            Config.Main.HideCompleted       = this.hideCompleted.Checked;
            Config.Main.RefreshIcon         = this.refreshIcon.Text.ToLower().Replace(" ", "_");
            Config.Main.LayoutDebug         = this.layoutDebug.Checked;
            Config.Main.RainbowMode         = this.theme.Text == "Pride Mode";
            Config.Main.BackColor           = ColorHelper.ToXNA(this.backColor.BackColor);
            Config.Main.TextColor           = ColorHelper.ToXNA(this.textColor.BackColor);
            Config.Main.BorderColor         = ColorHelper.ToXNA(this.borderColor.BackColor);
            Config.Main.Save();

            Config.Notes.Enabled = this.notesEnabled.Checked;
            Config.Notes.Save();
        }

        public void UpdateGameVersion()
        {
            this.gameVersion.Text = Config.Tracker.GameVersion;
            this.gameVersion.Enabled = !Peer.IsClient;
        }

        public void UpdateRainbow(Color color)
        {
            if (this.theme.Text.ToLower() is "pride mode")
            {
                this.backColor.BackColor   = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
                this.textColor.BackColor   = System.Drawing.Color.Black;
                this.borderColor.BackColor = System.Drawing.Color.FromArgb((int)(color.R / 1.25f), (int)(color.G / 1.25f), (int)(color.B / 1.25f));
            }
        }

        private void UpdateSaveGroupPanel()
        {
            if (this.worldLocal.Checked)
            {
                this.localGroup.Show();
                this.remoteGroup.Hide();
            }
            else
            {
                this.localGroup.Hide();
                this.remoteGroup.Show();
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
            else
            {
                using ColorDialog dialog = new () { Color = (sender as Control).BackColor};
                if (dialog.ShowDialog() is DialogResult.OK)
                {
                    (sender as Control).BackColor = dialog.Color;
                    if (sender == this.backColor ||
                        sender == this.textColor ||
                        sender == this.borderColor)
                    {
                        this.theme.SelectedItem = "Custom";
                    }
                }
            }
            this.SaveSettings();
        }

        private void OnCheckChanged(object sender, EventArgs e)
        {
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
                this.gameVersion.Enabled = !this.autoVersion.Checked && !Peer.IsClient;
            }
            this.SaveSettings();
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            if (sender == this.gameVersion)
            {
                Config.Tracker.TrySetGameVersion(this.gameVersion.Text);
            }
            else if (sender == this.theme)
            {
                if (this.theme.Text.ToLower() is not "custom")
                {
                    string themeName = this.theme.Text;
                    if (MainSettings.Themes.TryGetValue(themeName, out (Color, Color, Color) theme))
                    {
                        this.backColor.BackColor   = ColorHelper.ToDrawing(theme.Item1);
                        this.textColor.BackColor   = ColorHelper.ToDrawing(theme.Item2);
                        this.borderColor.BackColor = ColorHelper.ToDrawing(theme.Item3);
                    }
                    this.backColor.Enabled   = themeName is not "Pride Mode";
                    this.textColor.Enabled   = themeName is not "Pride Mode";
                    this.borderColor.Enabled = themeName is not "Pride Mode";
                }
                else
                {
                    this.backColor.BackColor         = ColorHelper.ToDrawing(Config.Main.BackColor);
                    this.textColor.BackColor         = ColorHelper.ToDrawing(Config.Main.TextColor);
                    this.borderColor.BackColor       = ColorHelper.ToDrawing(Config.Main.BorderColor);
                    Config.Main.RainbowMode = false;
                }
            }
            this.SaveSettings();
        }

        private void OnTextChanged(object sender, EventArgs e) 
        {
            this.SaveSettings();
        }
    }
}

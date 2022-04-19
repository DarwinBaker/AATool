using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.Winforms.Controls
{
    public partial class CMainSettings : UserControl
    {
        private bool loaded;

        public CMainSettings()
        {
            this.InitializeComponent();
        }

        public void LoadSettings()
        {
            this.loaded = false;

            this.fpsCap.Text            = Config.Main.FpsCap.Value.ToString();
            this.viewMode.Text          = Config.Main.CompactMode ? "Compact" : "Relaxed";
            this.showBasic.Checked      = Config.Main.ShowBasicAdvancements;
            this.hideCompleted.Checked  = Config.Main.HideCompletedAdvancements;
            this.completionGlow.Checked = Config.Main.ShowCompletionGlow;
            this.ambientGlow.Checked    = Config.Main.ShowAmbientGlow;
            this.highRes.Checked        = Config.Main.DisplayScale > 1;
            this.frameStyle.Text        = Config.Main.FrameStyle;
            this.progressBarStyle.Text  = Config.Main.ProgressBarStyle;
            this.refreshIcon.Text       = Config.Main.RefreshIcon;
            this.infoPanel.Text         = Config.Main.InfoPanel;
            this.backColor.BackColor    = ColorHelper.ToDrawing(Config.Main.BackColor);
            this.textColor.BackColor    = ColorHelper.ToDrawing(Config.Main.TextColor);
            this.borderColor.BackColor  = ColorHelper.ToDrawing(Config.Main.BorderColor);
            this.notesEnabled.Checked   = Config.Notes.Enabled;

            this.startupPosition.Text = Config.Main.StartupArrangement.Value.ToString();
            this.UpdateMonitorList();
            this.startupMonitor.SelectedIndex = MathHelper.Clamp(Config.Main.StartupDisplay - 1, 0, this.startupMonitor.Items.Count);
            this.startupMonitor.Enabled = this.startupPosition.Text != "Remember";

            //colors
            this.theme.Items.Clear();
            foreach (KeyValuePair<string, (Color, Color, Color)> theme in Config.MainConfig.Themes)
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

            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (this.loaded)
            {
                if (int.TryParse(this.fpsCap.Text, out int cap))
                    Config.Main.FpsCap.Set(cap);

                Config.Main.ShowBasicAdvancements.Set(this.showBasic.Checked);
                Config.Main.HideCompletedAdvancements.Set(this.hideCompleted.Checked);
                Config.Main.ShowCompletionGlow.Set(this.completionGlow.Checked);
                Config.Main.ShowAmbientGlow.Set(this.ambientGlow.Checked);
                Config.Main.DisplayScale.Set(this.highRes.Checked ? 2 : 1);
                Config.Main.CompactMode.Set(this.viewMode.Text.ToLower() is "compact");
                Config.Main.FrameStyle.Set(this.frameStyle.Text);
                Config.Main.ProgressBarStyle.Set(this.progressBarStyle.Text);
                Config.Main.RefreshIcon.Set(this.refreshIcon.Text);
                Config.Main.InfoPanel.Set(this.infoPanel.Text);
                Config.Main.RainbowMode.Set(this.theme.Text == "Pride Mode");
                Config.Main.BackColor.Set(ColorHelper.ToXNA(this.backColor.BackColor));
                Config.Main.TextColor.Set(ColorHelper.ToXNA(this.textColor.BackColor));
                Config.Main.BorderColor.Set(ColorHelper.ToXNA(this.borderColor.BackColor));

                Config.Main.StartupDisplay.Set(this.startupMonitor.SelectedIndex + 1);
                if (Enum.TryParse(this.startupPosition.Text, out WindowSnap position))
                    Config.Main.StartupArrangement.Set(position);

                Config.Main.Save();

                Config.Notes.Enabled.Set(this.notesEnabled.Checked);
                Config.Notes.Save();
            }
        }

        public void InvalidateSettings()
        {
            this.LoadSettings();
        }

        public void UpdateRainbow(Color color)
        {
            if (this.theme.Text.ToLower() is "pride mode")
            {
                this.backColor.BackColor = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
                this.textColor.BackColor = System.Drawing.Color.Black;
                this.borderColor.BackColor = System.Drawing.Color.FromArgb((int)(color.R / 1.25f), (int)(color.G / 1.25f), (int)(color.B / 1.25f));
            }
        }

        private void UpdateMonitorList()
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
                this.startupMonitor.Items.Add($"Display {i} ({Screen.AllScreens[i].Bounds.Width}x{Screen.AllScreens[i].Bounds.Height})");
        }

        private void OnClicked(object sender, EventArgs e)
        {
            using (var dialog = new ColorDialog() { Color = (sender as Control).BackColor })
            {
                if (dialog.ShowDialog() is DialogResult.OK)
                {
                    (sender as Control).BackColor = dialog.Color;
                    this.SaveSettings();
                    if (sender == this.backColor ||
                        sender == this.textColor ||
                        sender == this.borderColor)
                    {
                        this.theme.SelectedItem = "Custom";
                    }
                }
            }
        }

        private void OnCheckChanged(object sender, EventArgs e)
        {
            if (this.loaded)
                this.SaveSettings();
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            if (!this.loaded)
                return;

            if (sender == this.theme)
            {
                if (this.theme.Text.ToLower() is not "custom")
                {
                    string themeName = this.theme.Text;
                    if (Config.MainConfig.Themes.TryGetValue(themeName, out (Color, Color, Color) theme))
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
                    this.backColor.BackColor   = ColorHelper.ToDrawing(Config.Main.BackColor);
                    this.textColor.BackColor   = ColorHelper.ToDrawing(Config.Main.TextColor);
                    this.borderColor.BackColor = ColorHelper.ToDrawing(Config.Main.BorderColor);
                    Config.Main.RainbowMode.Set(false);
                }
            }
            else if (sender == this.startupPosition)
            {
                this.startupMonitor.Enabled = this.startupPosition.Text != "Remember";
            }
            this.SaveSettings();
        }
            
        private void OnTextChanged(object sender, EventArgs e) 
        {
            this.SaveSettings();
        }
    }
}

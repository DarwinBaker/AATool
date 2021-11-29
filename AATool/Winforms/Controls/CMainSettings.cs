using AATool.Net;
using AATool.Saves;
using AATool.Settings;
using AATool.UI;
using AATool.UI.Screens;
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
        }

        public void LoadSettings()
        {
            this.loaded = false;

            this.fpsCap.Text                = Config.Main.FpsCap.ToString();
            this.viewMode.Text              = Config.Main.CompactMode ? "Compact" : "Relaxed";
            this.showBasic.Checked          = Config.Main.ShowBasic;
            this.completionGlow.Checked     = Config.Main.CompletionGlow;
            this.hideCompleted.Checked      = Config.Main.HideCompleted;
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

            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (!this.loaded)
                return;

            if (int.TryParse(this.fpsCap.Text, out int fps))
                Config.Main.FpsCap = fps;

            Config.Main.CompactMode         = this.viewMode.Text.ToLower() is "compact";
            Config.Main.ShowBasic           = this.showBasic.Checked;
            Config.Main.CompletionGlow      = this.completionGlow.Checked;
            Config.Main.HideCompleted       = this.hideCompleted.Checked;
            Config.Main.RefreshIcon         = this.refreshIcon.Text.ToLower().Replace(" ", "_");
            Config.Main.RainbowMode         = this.theme.Text == "Pride Mode";
            Config.Main.BackColor           = ColorHelper.ToXNA(this.backColor.BackColor);
            Config.Main.TextColor           = ColorHelper.ToXNA(this.textColor.BackColor);
            Config.Main.BorderColor         = ColorHelper.ToXNA(this.borderColor.BackColor);
            Config.Main.Save();

            Config.Notes.Enabled = this.notesEnabled.Checked;
            Config.Notes.Save();
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

        private void OnClicked(object sender, EventArgs e)
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
                UIMainScreen.Invalidate();
            }
            this.SaveSettings();
        }

        private void OnCheckChanged(object sender, EventArgs e) => this.SaveSettings();

        private void OnIndexChanged(object sender, EventArgs e)
        {
            if (sender == this.theme)
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
                    this.backColor.BackColor   = ColorHelper.ToDrawing(Config.Main.BackColor);
                    this.textColor.BackColor   = ColorHelper.ToDrawing(Config.Main.TextColor);
                    this.borderColor.BackColor = ColorHelper.ToDrawing(Config.Main.BorderColor);
                    Config.Main.RainbowMode = false;
                }
            }
            this.SaveSettings();
        }

        private void OnTextChanged(object sender, EventArgs e) 
        {
            this.SaveSettings();
        }

        private void Label7_Click(object sender, EventArgs e)
        {

        }

        private void Label9_Click(object sender, EventArgs e)
        {

        }

        private void Label8_Click(object sender, EventArgs e)
        {

        }
    }
}

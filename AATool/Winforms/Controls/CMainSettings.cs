using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Data;
using AATool.Data.Speedrunning;
using AATool.Net;
using AATool.UI.Badges;
using AATool.Utilities;
using AATool.Winforms.Forms;
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

            this.UpdateBadgeList();
            this.UpdateFrameList();

            this.hideCompletedAdvancements.Checked = Config.Main.HideCompletedAdvancements;
            this.hideCompletedCriteria.Checked = Config.Main.HideCompletedCriteria;
            this.fpsCap.Text            = Config.Main.FpsCap.Value.ToString();
            this.viewMode.Text          = Main.TextInfo.ToTitleCase(Config.Main.Layout.Value);
            this.hideBasic.Checked      = !Config.Main.ShowBasicAdvancements;
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

                Config.Main.ShowBasicAdvancements.Set(!this.hideBasic.Checked);
                Config.Main.HideCompletedAdvancements.Set(this.hideCompletedAdvancements.Checked);
                Config.Main.HideCompletedCriteria.Set(this.hideCompletedCriteria.Checked);
                Config.Main.ShowAmbientGlow.Set(this.ambientGlow.Checked);
                Config.Main.DisplayScale.Set(this.highRes.Checked ? 2 : 1);
                Config.Main.Layout.Set(this.viewMode.Text.ToLower());
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

        public void UpdateBadgeList()
        {
            this.playerBadge.Items.Clear();
            this.playerBadge.Items.Add("Default");
            this.playerBadge.Items.Add("None");

            Uuid mainPlayer = Tracker.GetMainPlayer();
            _= Player.TryGetName(mainPlayer, out string name);

            if (Credits.TryGet(mainPlayer, out Credit supporter) || Credits.TryGet(name, out supporter))
            {
                if (supporter.Role is Credits.Developer || supporter.Uuid.String == Credits.Deadpool)
                {
                    this.playerBadge.Items.Add("Moderator");
                    this.playerBadge.Items.Add("VIP");
                }

                if (supporter.Role is Credits.NetheriteTier or Credits.Developer or Credits.BetaTester)
                {
                    this.playerBadge.Items.Add("Netherite");
                    this.playerBadge.Items.Add("Diamond");
                    this.playerBadge.Items.Add("Gold");
                }
                else if (supporter.Role is Credits.DiamondTier)
                {
                    this.playerBadge.Items.Add("Diamond");
                    this.playerBadge.Items.Add("Gold");
                }
                else if (supporter.Role is Credits.GoldTier)
                {
                    this.playerBadge.Items.Add("Gold");
                }
            }

            if (Badge.TryGet(mainPlayer, name, false, "All Advancements", "1.16", out Badge badge))
            {
                if (badge is not RankBadge && !this.playerBadge.Items.Contains(badge.GetListName))
                    this.playerBadge.Items.Insert(2, badge.GetListName);
            }

            if (!this.playerBadge.Items.Contains(Config.Main.PreferredPlayerBadge.Value))
                this.playerBadge.Items.Add(Config.Main.PreferredPlayerBadge.Value);
            this.playerBadge.Text = Config.Main.PreferredPlayerBadge.Value;

            if (!string.IsNullOrEmpty(name))
            {
                this.labelBadgeAvailability.Text = $"🛈 These are the badges and frames available to {name}.";
                if (supporter.Role is Credits.NetheriteTier)
                {
                    this.labelBadgeAvailability.Text += " Thanks for your incredible support!";
                }
                else if (supporter.Role is Credits.DiamondTier)
                {
                    this.labelBadgeAvailability.Text += " Upgrade to netherite tier for more!";
                }
                else if (supporter.Role is Credits.DiamondTier)
                {
                    this.labelBadgeAvailability.Text += " Upgrade to diamond or netherite tier for more!";
                }
            }
            else
            {
                this.labelBadgeAvailability.Text = $"🛈 More badges and frames are available to supporters of the AATool Patreon!";
            }
            

            /*
            for (int i = 0; i < this.playerBadge.Items.Count; i++)
            {
                if (this.playerBadge.Items[i] == Config.Main.PreferredPlayerBadge)
                {
                    this.playerBadge.SelectedIndex = i;
                    break;
                }
            }
            */
        }

        public void UpdateFrameList()
        {
            this.playerFrame.Items.Clear();
            this.playerFrame.Items.Add("Default");
            this.playerFrame.Items.Add("None");

            Uuid mainPlayer = Tracker.GetMainPlayer();
            _= Player.TryGetName(mainPlayer, out string name);

            if (Credits.TryGet(mainPlayer, out Credit supporter) || Credits.TryGet(name, out supporter))
            {
                if (supporter.Role is Credits.NetheriteTier or Credits.Developer or Credits.BetaTester)
                {
                    this.playerFrame.Items.Add("Netherite");
                    this.playerFrame.Items.Add("Diamond");
                    this.playerFrame.Items.Add("Gold");
                }
                else if (supporter.Role is Credits.DiamondTier)
                {
                    this.playerFrame.Items.Add("Diamond");
                    this.playerFrame.Items.Add("Gold");
                }
                else if (supporter.Role is Credits.GoldTier)
                {
                    this.playerFrame.Items.Add("Gold");
                }
            }

            if (!this.playerFrame.Items.Contains(Config.Main.PreferredPlayerFrame.Value))
                this.playerFrame.Items.Add(Config.Main.PreferredPlayerFrame.Value);
            this.playerFrame.Text = Config.Main.PreferredPlayerFrame.Value;
            /*
            for (int i = 0; i < this.playerFrame.Items.Count; i++)
            {
                if (this.playerFrame.Items[i] == Config.Main.PreferredPlayerFrame)
                {
                    this.playerFrame.SelectedIndex = i;
                    break;
                }
            }
            */
        }

        private void UpdateMonitorList()
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
                this.startupMonitor.Items.Add($"Display {i} ({Screen.AllScreens[i].Bounds.Width}x{Screen.AllScreens[i].Bounds.Height})");
        }

        private void OnClicked(object sender, EventArgs e)
        {
            if (sender == this.frameStyle)
            {
                using (var dialog = new FStyleDialog(false))
                {
                    dialog.ShowDialog();
                    this.frameStyle.Text = Config.Main.FrameStyle;
                    this.SaveSettings();
                }
            }
            else
            {
                using (var dialog = new ColorDialog() { Color = (sender as Control).BackColor })
                {
                    if (dialog.ShowDialog() is DialogResult.OK)
                    {
                        (sender as Control).BackColor = dialog.Color;
                        this.SaveSettings();
                        if (sender == this.backColor || sender == this.textColor || sender == this.borderColor)
                        {
                            this.theme.SelectedItem = "Custom";
                        }
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
            else if (sender == this.playerBadge)
            {
                Config.Main.PreferredPlayerBadge.Set(this.playerBadge.Text);
            }
            else if (sender == this.playerFrame)
            {
                Config.Main.PreferredPlayerFrame.Set(this.playerFrame.Text);
            }
            this.SaveSettings();
        }
            
        private void OnTextChanged(object sender, EventArgs e) 
        {
            this.SaveSettings();
        }
    }
}

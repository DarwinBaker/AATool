using System;
using System.Diagnostics;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Utilities;
using AATool.Winforms.Forms;
using Microsoft.Xna.Framework;

namespace AATool.Winforms.Controls
{
    public partial class COverlaySettings : UserControl
    {
        const int LayoutNormal = 0;
        const int LayoutOpposite = 1;
        const int LayoutOff = 2;

        private bool loaded;

        public COverlaySettings()
        {
            this.InitializeComponent();
        }

        private void UpdateMonitorList()
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
                this.startupMonitor.Items.Add($"Display {i} ({Screen.AllScreens[i].Bounds.Width}x{Screen.AllScreens[i].Bounds.Height})");
        }

        public void UpdateWidth() => this.overlayWidth.Value = Config.Overlay.Width;

        public void LoadSettings()
        {
            this.loaded = false;
            this.enabled.Checked      = Config.Overlay.Enabled;
            this.showText.Checked     = Config.Overlay.ShowLabels;
            this.showCriteria.Checked = Config.Overlay.ShowCriteria;
            this.showIgt.Checked      = Config.Overlay.ShowIgt;
            this.frameStyle.Text      = Config.Overlay.FrameStyle;
            this.clarifyAmbiguous.Checked = Config.Overlay.ClarifyAmbiguous;

            this.direction.SelectedIndex = Config.Overlay.RightToLeft ? LayoutNormal : LayoutOpposite;

            if (Config.Overlay.ShowPickups)
                this.pickupPosition.SelectedIndex = Config.Overlay.PickupsOpposite ? LayoutOpposite : LayoutNormal;
            else
                this.pickupPosition.SelectedIndex = LayoutOff;

            if (Config.Overlay.ShowLastRefresh)
                this.lastRefreshPosition.SelectedIndex = Config.Overlay.LastRefreshOpposite ? LayoutOpposite : LayoutNormal;
            else
                this.lastRefreshPosition.SelectedIndex = LayoutOff;

            this.overlayWidth.Value = Config.Overlay.Width;
            this.speed.Value = MathHelper.Clamp(Config.Overlay.Speed, this.speed.Minimum, this.speed.Maximum);

            this.greenscreenColor.BackColor = ColorHelper.ToDrawing(Config.Overlay.GreenScreen);
            this.textColor.BackColor = ColorHelper.ToDrawing(Config.Overlay.CustomTextColor);
            this.backColor.BackColor = ColorHelper.ToDrawing(Config.Overlay.CustomBackColor);
            this.borderColor.BackColor = ColorHelper.ToDrawing(Config.Overlay.CustomBorderColor);
            this.copyColorKey.Text   = $"Copy {ColorHelper.ToHexString(this.greenscreenColor.BackColor)}";
            this.copyColorKey.LinkColor = this.greenscreenColor.BackColor;
            this.restoreDefaultGreen.LinkColor = ColorHelper.ToDrawing(Config.Overlay.GreenScreen.Default);

            this.startupPosition.Text = Config.Overlay.StartupArrangement.Value.ToString();
            this.UpdateMonitorList();
            this.startupMonitor.SelectedIndex = MathHelper.Clamp(Config.Overlay.StartupDisplay - 1, 0, this.startupMonitor.Items.Count);
            this.startupMonitor.Enabled = this.startupPosition.Text != "Remember";
            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (!this.loaded)
                return;

            Config.Overlay.Enabled.Set(this.enabled.Checked);
            Config.Overlay.ShowLabels.Set(this.showText.Checked);
            Config.Overlay.ShowCriteria.Set(this.showCriteria.Checked);
            Config.Overlay.ShowIgt.Set(this.showIgt.Checked);
            Config.Overlay.ShowPickups.Set(this.pickupPosition.SelectedIndex is not LayoutOff);
            Config.Overlay.ShowLastRefresh.Set(this.lastRefreshPosition.SelectedIndex is not LayoutOff);
            Config.Overlay.ClarifyAmbiguous.Set(this.clarifyAmbiguous.Checked);
            Config.Overlay.Speed.Set(this.speed.Value);
            Config.Overlay.RightToLeft.Set(this.direction.SelectedIndex is LayoutNormal);
            Config.Overlay.PickupsOpposite.Set(this.pickupPosition.SelectedIndex is LayoutOpposite);
            Config.Overlay.LastRefreshOpposite.Set(this.lastRefreshPosition.SelectedIndex is LayoutOpposite);
            Config.Overlay.FrameStyle.Set(this.frameStyle.Text);
            Config.Overlay.Width.Set((int)this.overlayWidth.Value);
            Config.Overlay.GreenScreen.Set(ColorHelper.ToXNA(this.greenscreenColor.BackColor));
            Config.Overlay.CustomTextColor.Set(ColorHelper.ToXNA(this.textColor.BackColor));
            Config.Overlay.CustomBackColor.Set(ColorHelper.ToXNA(this.backColor.BackColor));
            Config.Overlay.CustomBorderColor.Set(ColorHelper.ToXNA(this.borderColor.BackColor));

            Config.Overlay.StartupDisplay.Set(this.startupMonitor.SelectedIndex + 1);
            if (Enum.TryParse(this.startupPosition.Text, out WindowSnap position))
                Config.Overlay.StartupArrangement.Set(position);

            Config.Overlay.Save();
        }

        private void OnClicked(object sender, EventArgs e)
        {
            if (sender == this.copyColorKey)
            {
                string r = $"{this.greenscreenColor.BackColor.R:X2}";
                string g = $"{this.greenscreenColor.BackColor.G:X2}";
                string b = $"{this.greenscreenColor.BackColor.B:X2}";
                Clipboard.SetText($"#{r}{g}{b}");
            }
            else if (sender == this.obsHelpLink)
            {
                Process.Start(Paths.Web.ObsHelp);
            }
            else if (sender == this.restoreDefaultGreen)
            {
                this.greenscreenColor.BackColor = ColorHelper.ToDrawing(Config.Overlay.GreenScreen.Default);
            }
            else if (sender == this.greenscreenColor 
                || sender == this.textColor 
                || sender == this.backColor 
                || sender == this.borderColor)
            {
                using ColorDialog dialog = new () { Color = (sender as Control).BackColor};
                if (dialog.ShowDialog() is DialogResult.OK)
                    (sender as Control).BackColor = dialog.Color; 
            }
            else if (sender == this.frameStyle)
            {
                using (var dialog = new FStyleDialog(true))
                {
                    dialog.ShowDialog();
                    this.frameStyle.Text = Config.Overlay.FrameStyle;
                }
            }
            this.SaveSettings();
        }

        private void OnCheckChanged(object sender, EventArgs e) => this.SaveSettings();
        private void OnValueChanged(object sender, EventArgs e) => this.SaveSettings();

        private void OnIndexChanged(object sender, EventArgs e)
        {
            if (sender == this.startupPosition)
                this.startupMonitor.Enabled = this.startupPosition.Text != "Remember";
            this.SaveSettings();
        }

        private void GreenscreenColorChanged(object sender, EventArgs e)
        {
            this.copyColorKey.LinkColor = this.greenscreenColor.BackColor;
            this.copyColorKey.Text = $"Copy {ColorHelper.ToHexString(this.greenscreenColor.BackColor)}";
        }
    }
}

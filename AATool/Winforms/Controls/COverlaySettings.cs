using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using AATool.Settings;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.Winforms.Controls
{
    public partial class COverlaySettings : UserControl
    {
        private bool loaded;
        private readonly TextInfo textInfo;

        public COverlaySettings()
        {
            this.InitializeComponent();
            this.textInfo = new CultureInfo("en-US", false).TextInfo;
        }

        public void UpdateWidth() => this.overlayWidth.Value = Config.Overlay.Width;

        public void LoadSettings()
        {
            this.loaded = false;
            this.enabled.Checked        = Config.Overlay.Enabled;
            this.showText.Checked       = Config.Overlay.ShowLabels;
            this.showCriteria.Checked   = Config.Overlay.ShowCriteria;
            this.showCounts.Checked     = Config.Overlay.ShowCounts;
            this.igt.Checked            = Config.Overlay.ShowIgt;

            this.direction.SelectedIndex    = Config.Overlay.RightToLeft ? 0 : 1;
            this.speed.Value                = MathHelper.Clamp(Config.Overlay.Speed, this.speed.Minimum, this.speed.Maximum);
            this.overlayWidth.Value         = Config.Overlay.Width;

            this.backColor.BackColor        = ColorHelper.ToDrawing(Config.Overlay.BackColor);
            this.textColor.BackColor        = ColorHelper.ToDrawing(Config.Overlay.TextColor);
            this.copyColorKey.Text          = $"Copy BG color {ColorHelper.ToHexString(this.backColor.BackColor)} for OBS";
            this.copyColorKey.LinkColor     = this.backColor.BackColor;
            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (!this.loaded)
                return;

            Config.Overlay.Enabled      = this.enabled.Checked;
            Config.Overlay.ShowLabels   = this.showText.Checked;
            Config.Overlay.ShowCriteria = this.showCriteria.Checked;
            Config.Overlay.ShowCounts   = this.showCounts.Checked;
            Config.Overlay.ShowIgt      = this.igt.Checked;
            Config.Overlay.Speed        = this.speed.Value;
            Config.Overlay.RightToLeft  = this.direction.SelectedIndex is 0;
            Config.Overlay.Width        = (int)this.overlayWidth.Value;
            Config.Overlay.BackColor    = ColorHelper.ToXNA(this.backColor.BackColor);
            Config.Overlay.TextColor    = ColorHelper.ToXNA(this.textColor.BackColor);
            Config.Overlay.Save();
        }

        private void OnClicked(object sender, EventArgs e)
        {
            if (sender == this.copyColorKey)
            {
                string r = $"{this.backColor.BackColor.R:X2}";
                string g = $"{this.backColor.BackColor.G:X2}";
                string b = $"{this.backColor.BackColor.B:X2}";
                Clipboard.SetText($"#{r}{g}{b}");
            }
            else if (sender == this.obsHelpLink)
            {
                Process.Start(Paths.URL_HELP_OBS);
            }
            else if (sender == this.backColor || sender == this.textColor)
            {
                using ColorDialog dialog = new () { Color = (sender as Control).BackColor};
                if (dialog.ShowDialog() is DialogResult.OK)
                    (sender as Control).BackColor = dialog.Color;
            }
            this.SaveSettings();
        }

        private void OnCheckChanged(object sender, EventArgs e) => this.SaveSettings();
        private void OnIndexChanged(object sender, EventArgs e) => this.SaveSettings();
        private void OnValueChanged(object sender, EventArgs e) => this.SaveSettings();
    }
}

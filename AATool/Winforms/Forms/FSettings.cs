using AATool.Settings;
using AATool.Trackers;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FSettings : Form
    {
        private TrackerSettings tracker = TrackerSettings.Instance;
        private MainSettings main       = MainSettings.Instance;
        private OverlaySettings overlay = OverlaySettings.Instance;

        private AdvancementTracker advancementTracker;

        public FSettings(AdvancementTracker advancementTracker)
        {
            InitializeComponent();
            LoadSettings();
            this.advancementTracker = advancementTracker;
        }

        public Color ToXNAColor(System.Drawing.Color color)     => new Color(color.R, color.G, color.B, color.A);
        public System.Drawing.Color ToDrawingColor(Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

        private void LoadSettings()
        {
            trackerUseDefault.Checked           = tracker.UseDefaultPath;
            trackerCustomSavesFolder.Text       = tracker.CustomPath;
            trackerRefreshDelay.Value           = tracker.RefreshInterval;

            mainShowBasic.Checked               = main.ShowBasic;
            mainRoundedCorners.Checked          = main.RenderRoundedCorners;
            mainBackColor.BackColor             = ToDrawingColor(main.BackColor);
            mainTextColor.BackColor             = ToDrawingColor(main.TextColor);
            mainBorderColor.BackColor           = ToDrawingColor(main.BorderColor);

            overlayEnabled.Checked              = overlay.Enabled;
            overlayHideCompleted.Checked        = overlay.HideCompleted;
            overlayOnlyShowFavorites.Checked    = overlay.OnlyShowFavorites;
            overlayShowText.Checked             = overlay.ShowLabels;
            overlayShowCriteria.Checked         = overlay.ShowCriteria;
            overlayShowCounts.Checked           = overlay.ShowCounts;
            overlayShowOverview.Checked         = overlay.ShowOverview;
            overlaySpeed.Value                  = MathHelper.Clamp(overlay.Speed, overlaySpeed.Minimum, overlaySpeed.Maximum);
            overlayWidth.Value                  = overlay.Width;
            overlayBackColor.BackColor          = ToDrawingColor(overlay.BackColor);
            overlayTextColor.BackColor          = ToDrawingColor(overlay.TextColor);

            mainTheme.Text = "Custom";
            if (main.RainbowMode)
                mainTheme.Text = "Pride Mode";
            else
            {
                foreach (var theme in MainSettings.Themes)
                {
                    if (main.BackColor == theme.Value.Item1 && main.TextColor == theme.Value.Item2 && main.BorderColor == theme.Value.Item3)
                    {
                        mainTheme.Text = theme.Key;
                        break;
                    }
                }
            }
        }

        private void SaveSettings()
        {
            tracker.UseDefaultPath      = trackerUseDefault.Checked;
            tracker.CustomPath          = trackerCustomSavesFolder.Text;
            tracker.RefreshInterval     = (int)trackerRefreshDelay.Value;
            tracker.Save();

            main.ShowBasic              = mainShowBasic.Checked;
            main.RenderRoundedCorners   = mainRoundedCorners.Checked;
            main.RainbowMode            = mainTheme.Text == "Pride Mode";
            main.BackColor              = ToXNAColor(mainBackColor.BackColor);
            main.TextColor              = ToXNAColor(mainTextColor.BackColor);
            main.BorderColor            = ToXNAColor(mainBorderColor.BackColor);
            main.Save();

            overlay.Enabled             = overlayEnabled.Checked;
            overlay.HideCompleted       = overlayHideCompleted.Checked;
            overlay.OnlyShowFavorites   = overlayOnlyShowFavorites.Checked;
            overlay.ShowLabels            = overlayShowText.Checked;
            overlay.ShowCriteria        = overlayShowCriteria.Checked;
            overlay.ShowCounts          = overlayShowCounts.Checked;
            overlay.ShowOverview        = overlayShowOverview.Checked;
            overlay.Speed               = overlaySpeed.Value;
            overlay.Width               = (int)overlayWidth.Value;
            overlay.BackColor           = ToXNAColor(overlayBackColor.BackColor);
            overlay.TextColor           = ToXNAColor(overlayTextColor.BackColor);
            overlay.Save();
        }

        public void UpdateRainbow(Color color)
        {
            if (mainTheme.Text == "Pride Mode")
            {
                mainBackColor.BackColor = System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
                mainTextColor.BackColor = System.Drawing.Color.Black;
                mainBorderColor.BackColor = System.Drawing.Color.FromArgb(255, (int)(color.R / 1.25f), (int)(color.G / 1.25f), (int)(color.B / 1.25f));
            }
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (sender == trackerBrowse)
            {
                using (var dialog = new FolderBrowserDialog())
                    if (dialog.ShowDialog() == DialogResult.OK)
                        trackerCustomSavesFolder.Text = dialog.SelectedPath;
            }
            else if (sender == save || sender == cancel)
            {
                if (sender == save)
                    SaveSettings();
                Close();
            }
            else if (sender == defaults)
            {
                string msg = "This will clear all customized settings, including user marked favorite advancements and your custom save path. "
                           + "These cannot be recovered. Are you sure you want to reset?";
                if (MessageBox.Show(this, msg, "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    main.ResetToDefaults();
                    main.Save();
                    tracker.ResetToDefaults();
                    tracker.Save();
                    overlay.ResetToDefaults();
                    overlay.Save();
                    Close();
                }
            }
            else if (sender == about)
            {
                using (var dialog = new FAbout())
                    dialog.ShowDialog();
            }
            else if (sender == overlayPickFavorites)
            {
                using (var dialog = new FPickFavorites(advancementTracker))
                    dialog.ShowDialog();
            }
            else
            {
                using (var dialog = new ColorDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        (sender as Control).BackColor = dialog.Color;
                        if (sender == mainBackColor || sender == mainTextColor || sender == mainBorderColor)
                            mainTheme.SelectedItem = "Custom";
                    }
                }
            }
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (sender == trackerUseDefault)
                trackerCustomSavesFolder.Enabled = !trackerUseDefault.Checked;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (sender == mainTheme)
            {
                string themeName = mainTheme.Text;
                if (MainSettings.Themes.TryGetValue(themeName, out var theme))
                {
                    mainBackColor.BackColor = ToDrawingColor(theme.Item1);
                    mainTextColor.BackColor = ToDrawingColor(theme.Item2);
                    mainBorderColor.BackColor = ToDrawingColor(theme.Item3);
                }
                mainBackColor.Enabled = themeName != "Pride Mode";
                mainTextColor.Enabled = themeName != "Pride Mode";
                mainBorderColor.Enabled = themeName != "Pride Mode";
            }
        }
    }
}

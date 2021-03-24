using AATool.Settings;
using AATool.Trackers;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FSettings : Form
    {
        private TrackerSettings tracker = TrackerSettings.Instance;
        private MainSettings main       = MainSettings.Instance;
        private OverlaySettings overlay = OverlaySettings.Instance;

        private AdvancementTracker advancementTracker;
        private AchievementTracker achievementTracker;
        private StatisticsTracker statisticsTracker;

        public FSettings(Form parent, AdvancementTracker advancementTracker, AchievementTracker achievementTracker, StatisticsTracker statisticsTracker)
        {
            InitializeComponent();
            LoadSettings();
            this.advancementTracker = advancementTracker;
            this.achievementTracker = achievementTracker;
            this.statisticsTracker  = statisticsTracker;

            trackerGameVersion.Items.Clear();
            foreach (var version in TrackerSettings.SupportedVersions.Reverse())
                trackerGameVersion.Items.Add(version);
            trackerGameVersion.Text = tracker.GameVersion;
        }

        public Color ToXNAColor(System.Drawing.Color color)     => new Color(color.R, color.G, color.B, color.A);
        public System.Drawing.Color ToDrawingColor(Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CenterToParent();
        }

        private void LoadSettings()
        {
            trackerUseDefault.Checked           = tracker.UseDefaultPath;
            trackerCustomSavesFolder.Text       = tracker.CustomPath;
            trackerGameVersion.Text             = tracker.GameVersion;
            trackerAutoVersion.Checked          = tracker.AutoDetectVersion;
            trackerRefreshDelay.Value           = tracker.RefreshInterval;

            mainShowBasic.Checked               = main.ShowBasic;
            mainFancyCorners.Checked            = main.RenderFancyCorners;
            mainCompletionGlow.Checked          = main.RenderCompletionGlow;
            mainLayoutDebug.Checked             = main.LayoutDebug;
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
            overlayDirection.SelectedIndex = overlay.RightToLeft ? 0 : 1;
            overlaySpeed.Value                  = MathHelper.Clamp(overlay.Speed, overlaySpeed.Minimum, overlaySpeed.Maximum);
            overlayWidth.Value                  = overlay.Width;
            overlayBackColor.BackColor          = ToDrawingColor(overlay.BackColor);
            overlayTextColor.BackColor          = ToDrawingColor(overlay.TextColor);
            copyColorKey.Text                   = "Copy BG color " + $"#{overlayBackColor.BackColor.R:X2}{overlayBackColor.BackColor.G:X2}{overlayBackColor.BackColor.B:X2}" + " for OBS";
            copyColorKey.LinkColor              = overlayBackColor.BackColor;

            mainTheme.Items.Clear();
            foreach (var theme in MainSettings.Themes)
            {
                mainTheme.Items.Add(theme.Key);
                if (main.BackColor == theme.Value.Item1 && main.TextColor == theme.Value.Item2 && main.BorderColor == theme.Value.Item3)
                    mainTheme.Text = theme.Key;
            }
            mainTheme.Items.Add("Pride Mode");
            mainTheme.Items.Add("Custom");
            if (main.RainbowMode)
                mainTheme.Text = "Pride Mode";
            else if (string.IsNullOrEmpty(mainTheme.Text))
                mainTheme.Text = "Custom";
        }

        private void SaveSettings()
        {
            tracker.UseDefaultPath      = trackerUseDefault.Checked;
            tracker.CustomPath          = trackerCustomSavesFolder.Text;
            tracker.AutoDetectVersion   = trackerAutoVersion.Checked;
            tracker.RefreshInterval     = (int)trackerRefreshDelay.Value;
            tracker.TrySetGameVersion(trackerGameVersion.Text);
            tracker.Save();

            main.ShowBasic              = mainShowBasic.Checked;
            main.RenderFancyCorners     = mainFancyCorners.Checked;
            main.RenderCompletionGlow   = mainCompletionGlow.Checked;
            main.LayoutDebug            = mainLayoutDebug.Checked;
            main.RainbowMode            = mainTheme.Text == "Pride Mode";
            main.BackColor              = ToXNAColor(mainBackColor.BackColor);
            main.TextColor              = ToXNAColor(mainTextColor.BackColor);
            main.BorderColor            = ToXNAColor(mainBorderColor.BackColor);
            main.Save();

            overlay.Enabled             = overlayEnabled.Checked;
            overlay.HideCompleted       = overlayHideCompleted.Checked;
            overlay.OnlyShowFavorites   = overlayOnlyShowFavorites.Checked;
            overlay.ShowLabels          = overlayShowText.Checked;
            overlay.ShowCriteria        = overlayShowCriteria.Checked;
            overlay.ShowCounts          = overlayShowCounts.Checked;
            overlay.ShowOverview        = overlayShowOverview.Checked;
            overlay.Speed               = overlaySpeed.Value;
            overlay.RightToLeft         = overlayDirection.SelectedIndex == 0;
            overlay.Width               = (int)overlayWidth.Value;
            overlay.BackColor           = ToXNAColor(overlayBackColor.BackColor);
            overlay.TextColor           = ToXNAColor(overlayTextColor.BackColor);
            overlay.Save();
        }

        public void UpdateGameVersion()
        {
            trackerGameVersion.Text = tracker.GameVersion;
        }

        public void UpdateRainbow(Color color)
        {
            if (mainTheme.Text == "Pride Mode")
            {
                mainBackColor.BackColor   = System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
                mainTextColor.BackColor   = System.Drawing.Color.Black;
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
                string msg = "This will clear all customized settings, including user marked favorites and your custom save path. "
                           + "Are you sure you want to revert to the default settings?";
                if (MessageBox.Show(this, msg, "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    main.ResetToDefaults();
                    main.Save();
                    tracker.ResetToDefaults();
                    tracker.Save();
                    overlay.ResetToDefaults();
                    overlay.Save();
                    Directory.Delete(Paths.DIR_FAVORITES, true);
                    Close();
                }
            }
            else if (sender == update)
                Main.CheckForUpdatesAsync(false);
            else if (sender == about)
                using (var dialog = new FAbout())
                    dialog.ShowDialog();
            else if (sender == overlayPickFavorites)
                using (var dialog = new FPickFavorites(advancementTracker, achievementTracker, statisticsTracker))
                    dialog.ShowDialog();
            else if (sender == copyColorKey)
                Clipboard.SetText($"#{overlayBackColor.BackColor.R:X2}{overlayBackColor.BackColor.G:X2}{overlayBackColor.BackColor.B:X2}");
            else
            {
                using (var dialog = new ColorDialog())
                {
                    dialog.Color = (sender as Control).BackColor;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        (sender as Control).BackColor = dialog.Color;
                        if (sender == mainBackColor || sender == mainTextColor || sender == mainBorderColor)
                            mainTheme.SelectedItem = "Custom";
                        else if (sender == overlayBackColor)
                        {
                            copyColorKey.Text = "Copy BG color " + $"#{overlayBackColor.BackColor.R:X2}{overlayBackColor.BackColor.G:X2}{overlayBackColor.BackColor.B:X2}" + " for OBS";
                            copyColorKey.LinkColor = dialog.Color;
                        }
                    }
                }
            }
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (sender == trackerUseDefault)
                trackerCustomSavesFolder.Enabled = !trackerUseDefault.Checked;
            else if (sender == trackerAutoVersion)
                trackerGameVersion.Enabled = !trackerAutoVersion.Checked;
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            if (sender == trackerGameVersion)
            {
                tracker.TrySetGameVersion(trackerGameVersion.Text);
                overlay.LoadFavorites();
            }
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

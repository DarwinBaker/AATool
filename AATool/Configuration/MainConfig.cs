using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AATool.Data.Categories;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace AATool.Configuration
{
    public abstract partial class Config
    {
        private static readonly Version VerticalMonitorUpdate = new ("1.4.5.0");

        public const string RelaxedLayout = "relaxed";
        public const string CompactLayout = "compact";
        public const string VerticalLayout = "vertical";
        public const string OptimizedLayout = "optimized";

        public const string AutoSwitchPanel = "Auto-Switch";
        public const string RunOverviewPanel = "Run Overview";
        public const string LeaderboardPanel = "Leaderboard";
        public const string BrewingPanel = "Potion Recipes";
        

        [JsonObject]
        public class MainConfig : Config
        {
            public static readonly Dictionary<string, (Color back, Color text, Color border)> Themes = new ()
            {
                {"Dark Mode",      (Hex("36393F"), Hex("DCDDDE"), Hex("4E5156"))},
                {"Light Mode",     (Hex("F0F0F0"), Hex("000000"), Hex("C4C4C4"))},
                {"GitHub Dark",    (Hex("0D1117"), Hex("C9D1D9"), Hex("30363D"))},
                {"Ender Pearl",    (Hex("0C3730"), Hex("C6F2EA"), Hex("349988"))},
                {"Blazed",         (Hex("91360B"), Hex("FFFFCC"), Hex("E4871F"))},
                {"Brick",          (Hex("804040"), Hex("FFFFFF"), Hex("AA5A5A"))},
                {"Berry",          (Hex("411126"), Hex("C9D1D9"), Hex("5E1938"))},
                {"Couri Enjoyer",  (Hex("502880"), Hex("C9D1D9"), Hex("8336B6"))},
                {"90's Hacker",    (Hex("001800"), Hex("00FF00"), Hex("005000"))},
                {"High Contrast",  (Hex("000000"), Hex("FFFFFF"), Hex("FFFFFF"))}
            };

            private static Color Hex(string hex) => 
                ColorHelper.TryGetHexColor(hex, out Color color) ? color : Color.White;

            [JsonProperty] public readonly Setting<int> FpsCap = new (60);
            [JsonProperty] public readonly Setting<int> DisplayScale = new (1);

            [JsonProperty] public readonly Setting<bool> AllowUserResizing = new (false);
            [JsonProperty] public readonly Setting<bool> HideCompletedAdvancements = new (false);
            [JsonProperty] public readonly Setting<bool> HideCompletedCriteria = new (false);
            [JsonProperty] public readonly Setting<bool> ShowBasicAdvancements = new (true);
            [JsonProperty] public readonly Setting<bool> ShowAmbientGlow = new (true);
            [JsonProperty] public readonly Setting<bool> ShowMyBadge = new (true);
            [JsonProperty] public readonly Setting<bool> RainbowMode = new (false);
            [JsonProperty] public readonly Setting<bool> CloseFramesOnSelection = new (true);

            [JsonProperty] public readonly Setting<string> Layout = new (MonitorSupportsRelaxed ? RelaxedLayout : CompactLayout);

            [JsonProperty] public readonly Setting<bool> LayoutDebugMode = new (false);
            [JsonProperty] public readonly Setting<bool> CacheDebugMode = new (false);
            [JsonProperty] public readonly Setting<bool> HideRenderCache = new (false);
            [JsonProperty] public readonly Setting<bool> HideGlowEffects = new (false);

            [JsonProperty] public readonly Setting<string> FrameStyle = new ("Modern");
            [JsonProperty] public readonly Setting<string> PrideFrameList = new (string.Empty);
            [JsonProperty] public readonly Setting<string> ProgressBarStyle = new ("Modern");
            [JsonProperty] public readonly Setting<string> RefreshIcon = new ("Xp Orb");
            [JsonProperty] public readonly Setting<string> InfoPanel = new ("Leaderboard");

            [JsonProperty] public readonly Setting<string> PreferredPlayerBadge = new ("Default");
            [JsonProperty] public readonly Setting<string> PreferredPlayerFrame = new ("Default");

            [JsonProperty] public readonly Setting<Color> BackColor = new (Hex("36393F"));
            [JsonProperty] public readonly Setting<Color> TextColor = new (Hex("DCDDDE"));
            [JsonProperty] public readonly Setting<Color> BorderColor = new (Hex("4E5156"));

            [JsonProperty] public readonly Setting<WindowSnap> StartupArrangement = new (WindowSnap.Centered);
            [JsonProperty] public readonly Setting<Point> LastWindowPosition = new (Point.Zero);
            [JsonProperty] public readonly Setting<int> StartupDisplay = new (1);
            [JsonProperty] public readonly Setting<bool> AlwaysOnTop = new (false);

            //deprecated (now used to migrate preference from pre-1.4.5.0)
            [JsonProperty] public readonly Setting<bool> CompactMode = new (false);

            [JsonIgnore]
            public bool UseCompactStyling => (this.Layout.Value is CompactLayout || this.UseOptimizedLayout)
                && Tracker.Category is not AllBlocks;

            [JsonIgnore]
            public bool UseVerticalStyling => this.Layout.Value is VerticalLayout;

            [JsonIgnore]
            public bool UseOptimizedLayout => this.Layout.Value is OptimizedLayout
                && Tracker.Category?.GetType() == typeof(AllAdvancements);

            [JsonIgnore]
            public bool AppearanceChanged => this.FrameStyle.Changed
                || this.BorderColor.Changed
                || this.BackColor.Changed
                || this.TextColor.Changed
                || this.ProgressBarStyle.Changed
                || this.PrideFrameList.Changed;

            [JsonIgnore]
            private static bool MonitorSupportsRelaxed =>
                Screen.PrimaryScreen.Bounds.Width >= 1600
                && Screen.PrimaryScreen.Bounds.Height >= 900;

            [JsonIgnore] private string[] prideStyles;

            protected override string GetId() => "main";
            protected override string GetLegacyId() => "main";

            public MainConfig()
            {
                this.RegisterSetting(this.Layout);
                this.RegisterSetting(this.FpsCap);
                this.RegisterSetting(this.DisplayScale);

                this.RegisterSetting(this.AllowUserResizing);

                this.RegisterSetting(this.HideCompletedAdvancements);
                this.RegisterSetting(this.HideCompletedCriteria);

                this.RegisterSetting(this.ShowBasicAdvancements);
                this.RegisterSetting(this.ShowAmbientGlow);
                this.RegisterSetting(this.ShowMyBadge);

                this.RegisterSetting(this.CompactMode);
                this.RegisterSetting(this.RainbowMode);

                this.RegisterSetting(this.LayoutDebugMode);
                this.RegisterSetting(this.CacheDebugMode);
                this.RegisterSetting(this.HideGlowEffects);

                this.RegisterSetting(this.FrameStyle);
                this.RegisterSetting(this.PrideFrameList);
                this.RegisterSetting(this.ProgressBarStyle);
                this.RegisterSetting(this.RefreshIcon);
                this.RegisterSetting(this.InfoPanel);

                this.RegisterSetting(this.PreferredPlayerBadge);
                this.RegisterSetting(this.PreferredPlayerFrame);

                this.RegisterSetting(this.BackColor);
                this.RegisterSetting(this.TextColor);
                this.RegisterSetting(this.BorderColor);

                this.RegisterSetting(this.StartupArrangement);
                this.RegisterSetting(this.StartupDisplay);
                this.RegisterSetting(this.LastWindowPosition);
                this.RegisterSetting(this.AlwaysOnTop);
            }

            public void SetPrideList(string csv)
            {
                this.PrideFrameList.Set(csv);
                this.prideStyles = csv.Split(',');
            }

            public string GetActiveFrameStyle(int x, int y)
            {
                int col = x / (this.Layout == CompactLayout ? 60 : 68);
                int row = y / (this.Layout == CompactLayout ? 72 : 84);
                this.prideStyles ??= this.PrideFrameList.Value.Split(',');
                if (this.FrameStyle == "Multi-Pride" && this.prideStyles.Any())
                {
                    int index = (col + row) % this.prideStyles.Length;
                    return this.prideStyles[index];
                }
                else
                {
                    return this.FrameStyle;
                }
            }

            protected override void MigrateDepricatedConfigs()
            {
                //prevent some settings from being blank
                if (string.IsNullOrWhiteSpace(this.Layout))
                    this.Layout.ApplyDefault();
                if (string.IsNullOrWhiteSpace(this.FrameStyle))
                    this.FrameStyle.ApplyDefault();
                if (string.IsNullOrWhiteSpace(this.ProgressBarStyle))
                    this.ProgressBarStyle.ApplyDefault();
                if (string.IsNullOrWhiteSpace(this.InfoPanel))
                    this.InfoPanel.ApplyDefault();
                if (string.IsNullOrWhiteSpace(this.RefreshIcon))
                    this.RefreshIcon.ApplyDefault();

                //migrate relaxed vs compact preference from pre-1.4.5.0 installation
                if (Version.TryParse(Tracking.LastSession, out Version last)
                    && last < VerticalMonitorUpdate && this.CompactMode)
                {
                    this.Layout.Set(CompactLayout);
                    this.CompactMode.Set(false);
                    this.TrySave();
                }
            }

            protected override void ApplyLegacySetting(string key, object value)
            {
                ISetting setting = key switch {
                    "show_basic_advancements" => this.ShowBasicAdvancements,
                    "fps_cap"           => this.FpsCap,
                    "layout_debug"      => this.LayoutDebugMode,
                    "hide_completed"    => this.HideCompletedAdvancements,
                    "refresh_icon"      => this.RefreshIcon,
                    "rainbow_mode"      => this.RainbowMode,
                    "main_back_color"   => this.BackColor,
                    "main_border_color" => this.BorderColor,
                    "main_text_color"   => this.TextColor,
                    _ => null
                };
                setting?.Set(value);
            }
        }
    }
}

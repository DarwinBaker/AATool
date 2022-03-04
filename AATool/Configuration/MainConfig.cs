using AATool.Utilities;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace AATool.Configuration
{
    public abstract partial class Config
    {
        [JsonObject]
        public class MainConfig : Config
        {
            public static readonly Dictionary<string, (Color back, Color text, Color border)> Themes = new ()
            {
                {"Dark Mode",      (Hex("36393F"), Hex("DCDDDE"), Hex("4E5156"))},
                {"Light Mode",     (Hex("F0F0F0"), Hex("000000"), Hex("C4C4C4"))},
                {"GitHub Dark",    (Hex("0D1117"), Hex("C9D1D9"), Hex("30363D"))},
                {"Ender Pearl",    (Hex("0C3730"), Hex("C6F2EA"), Hex("349988"))},
                {"Brick",          (Hex("804040"), Hex("FFFFFF"), Hex("AA5A5A"))},
                {"Berry",          (Hex("411126"), Hex("C9D1D9"), Hex("5E1938"))},
                {"Couri Enjoyer",  (Hex("502880"), Hex("C9D1D9"), Hex("8336B6"))},
                {"90's Hacker",    (Hex("001800"), Hex("00FF00"), Hex("005000"))},
                {"High Contrast",  (Hex("000000"), Hex("FFFFFF"), Hex("FFFFFF"))}
            };

            private static Color Hex(string hex) => ColorHelper.TryGetHexColor(hex, out Color color) ? color : Color.White;

            [JsonProperty] public readonly Setting<int> FpsCap = new (60);
            [JsonProperty] public readonly Setting<int> DisplayScale = new (1);

            [JsonProperty] public readonly Setting<bool> HideCompletedAdvancements = new (false);
            [JsonProperty] public readonly Setting<bool> ShowBasicAdvancements = new (true);
            [JsonProperty] public readonly Setting<bool> ShowCompletionGlow = new (true);
            [JsonProperty] public readonly Setting<bool> ShowAmbientGlow = new (true);
            [JsonProperty] public readonly Setting<bool> CompactMode = new (false);
            [JsonProperty] public readonly Setting<bool> RainbowMode = new (false);

            [JsonProperty] public readonly Setting<bool> LayoutDebugMode = new (false);
            [JsonProperty] public readonly Setting<bool> CacheDebugMode = new (false);
            [JsonProperty] public readonly Setting<bool> HideRenderCache = new (false);

            [JsonProperty] public readonly Setting<string> FrameStyle = new ("Modern");
            [JsonProperty] public readonly Setting<string> ProgressBarStyle = new ("Modern");
            [JsonProperty] public readonly Setting<string> RefreshIcon = new ("Xp Orb");
            [JsonProperty] public readonly Setting<string> InfoPanel = new ("Leaderboard");

            [JsonProperty] public readonly Setting<Color> BackColor = new (Hex("36393F"));
            [JsonProperty] public readonly Setting<Color> TextColor = new (Hex("DCDDDE"));
            [JsonProperty] public readonly Setting<Color> BorderColor = new (Hex("4E5156"));

            public bool RelaxedMode => !this.CompactMode;
            public string ViewMode => this.CompactMode ? "compact" : "relaxed";

            public bool StyleChanged => this.FrameStyle.Changed
                || this.BorderColor.Changed
                || this.BackColor.Changed
                || this.TextColor.Changed
                || this.ProgressBarStyle.Changed;

            protected override string GetId() => "main";
            protected override string GetLegacyId() => "main";

            public MainConfig()
            {
                this.RegisterSetting(this.FpsCap);
                this.RegisterSetting(this.DisplayScale);
                this.RegisterSetting(this.ShowBasicAdvancements);
                this.RegisterSetting(this.ShowCompletionGlow);
                this.RegisterSetting(this.CompactMode);
                this.RegisterSetting(this.RainbowMode);
                this.RegisterSetting(this.LayoutDebugMode);
                this.RegisterSetting(this.FrameStyle);
                this.RegisterSetting(this.ProgressBarStyle);
                this.RegisterSetting(this.RefreshIcon);
                this.RegisterSetting(this.InfoPanel);
                this.RegisterSetting(this.BackColor);
                this.RegisterSetting(this.TextColor);
                this.RegisterSetting(this.BorderColor);
            } 

            protected override void ApplyLegacySetting(string key, object value)
            {
                ISetting setting = key switch {
                    "show_basic_advancements" => this.ShowBasicAdvancements,
                    "fps_cap"           => this.FpsCap,
                    "layout_debug"      => this.LayoutDebugMode,
                    "compact_mode"      => this.CompactMode,
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

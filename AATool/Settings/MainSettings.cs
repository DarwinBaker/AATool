using AATool.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class MainSettings : SettingsGroup
    {
        public static MainSettings Instance = new ();

        public const string FPS_CAP         = "fps_cap";
        public const string SHOW_BASIC      = "show_basic_advancements";
        public const string COMPLETION_GLOW = "completion_glow";
        public const string LAYOUT_DEBUG    = "layout_debug";
        public const string COMPACT_MODE    = "compact_mode";
        public const string HIDE_COMPLETED  = "hide_completed";
        public const string REFRESH_ICON    = "refresh_icon";
        public const string RAINBOW_MODE    = "rainbow_mode";
        public const string BACK_COLOR      = "main_back_color";
        public const string TEXT_COLOR      = "main_text_color";
        public const string BORDER_COLOR    = "main_border_color";

        public int FpsCap                   { get => this.Get<int>(FPS_CAP);          set => this.Set(FPS_CAP, value); }
        public bool ShowBasic               { get => this.Get<bool>(SHOW_BASIC);      set => this.Set(SHOW_BASIC, value); }
        public bool CompletionGlow          { get => this.Get<bool>(COMPLETION_GLOW); set => this.Set(COMPLETION_GLOW, value); }
        public bool RainbowMode             { get => this.Get<bool>(RAINBOW_MODE);    set => this.Set(RAINBOW_MODE, value); }
        public bool CompactMode             { get => this.Get<bool>(COMPACT_MODE);    set => this.Set(COMPACT_MODE, value); }
        public bool HideCompleted           { get => this.Get<bool>(HIDE_COMPLETED);  set => this.Set(HIDE_COMPLETED, value); }
        public bool LayoutDebug             { get => this.Get<bool>(LAYOUT_DEBUG);    set => this.Set(LAYOUT_DEBUG, value); }
        public string RefreshIcon           { get => this.Get<string>(REFRESH_ICON);  set => this.Set(REFRESH_ICON, value); }
        public Color BackColor              { get => this.Get<Color>(BACK_COLOR);     set => this.Set(BACK_COLOR, value); }
        public Color TextColor              { get => this.Get<Color>(TEXT_COLOR);     set => this.Set(TEXT_COLOR, value); }
        public Color BorderColor            { get => this.Get<Color>(BORDER_COLOR);   set => this.Set(BORDER_COLOR, value); }

        public bool FpsCapChanged() => Instance.ValueChanged(FPS_CAP);

        private static Color Hex(string hex) => 
            ColorHelper.TryGetHexColor(hex, out Color color) ? color : Color.White;

        public static readonly Dictionary<string, (Color back, Color text, Color border)> Themes = new ()
        {
            //theme presets
            {"Dark Mode",      (Hex("36393F"), Hex("DCDDDE"), Hex("4E5156"))},
            {"Light Mode",     (Hex("F0F0F0"), Hex("000000"), Hex("C4C4C4"))},

            {"GitHub Dark",    (Hex("0D1117"), Hex("C9D1D9"), Hex("30363D"))},

            {"Ender Pearl",    (Hex("0C3730"), Hex("C6F2EA"), Hex("349988"))},
            {"Blaze Rod",      (Hex("953300"), Hex("FFF87E"), Hex("FFC100"))},
            
            {"Brick",          (Hex("804040"), Hex("FFFFFF"), Hex("AA5A5A"))},
            {"Berry",          (Hex("411126"), Hex("C9D1D9"), Hex("5E1938"))},
            {"90's Hacker",    (Hex("001800"), Hex("00FF00"), Hex("005000"))},
            {"High Contrast",  (Hex("000000"), Hex("FFFFFF"), Hex("FFFFFF"))}
        };

        private MainSettings()
        {
            this.Load("main");
        }

        public override void ResetToDefaults()
        {
            this.Set(SHOW_BASIC,        true);
            this.Set(COMPLETION_GLOW,   true);
            this.Set(RAINBOW_MODE,      false);
            this.Set(COMPACT_MODE,      false);
            this.Set(HIDE_COMPLETED,    false);
            this.Set(REFRESH_ICON,      "xp_orb");
            this.Set(LAYOUT_DEBUG,      false);
            this.Set(BACK_COLOR,        Hex("36393F"));
            this.Set(TEXT_COLOR,        Hex("DCDDDE"));
            this.Set(BORDER_COLOR,      Hex("4E5156"));
        }
    }
}

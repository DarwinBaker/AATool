using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class MainSettings : SettingsGroup
    {
        public static MainSettings Instance = new MainSettings();

        public const string SHOW_BASIC      = "show_basic_advancements";
        public const string FANCY_CORNERS   = "fancy_corners";
        public const string COMPLETION_GLOW = "completion_glow";
        public const string LAYOUT_DEBUG    = "layout_debug";
        public const string RAINBOW_MODE    = "rainbow_mode";
        public const string BACK_COLOR      = "main_back_color";
        public const string TEXT_COLOR      = "main_text_color";
        public const string BORDER_COLOR    = "main_border_color";
        
        public bool ShowBasic               { get => Get<bool>(SHOW_BASIC);      set => Set(SHOW_BASIC, value); }
        public bool RenderFancyCorners      { get => Get<bool>(FANCY_CORNERS);   set => Set(FANCY_CORNERS, value); }
        public bool RenderCompletionGlow    { get => Get<bool>(COMPLETION_GLOW); set => Set(COMPLETION_GLOW, value); }
        public bool RainbowMode             { get => Get<bool>(RAINBOW_MODE);    set => Set(RAINBOW_MODE, value); }
        public bool LayoutDebug             { get => Get<bool>(LAYOUT_DEBUG);    set => Set(LAYOUT_DEBUG, value); }
        public Color BackColor              { get => Get<Color>(BACK_COLOR);     set => Set(BACK_COLOR, value); }
        public Color TextColor              { get => Get<Color>(TEXT_COLOR);     set => Set(TEXT_COLOR, value); }
        public Color BorderColor            { get => Get<Color>(BORDER_COLOR);   set => Set(BORDER_COLOR, value); }

        private static Color ColorOf(int r, int g, int b) => Color.FromNonPremultiplied(r, g, b, 255);

        public static readonly Dictionary<string, (Color, Color, Color)> Themes = new Dictionary<string, (Color, Color, Color)>()
        {
            //establish theme presets
            { "Light Mode",     (ColorOf(240, 240, 240), Color.Black,            ColorOf(196, 196, 196)  )},
            { "Dark Mode",      (ColorOf(64, 64, 64),    Color.White,            ColorOf(128, 128, 128)  )},
            { "Brick",          (ColorOf(128, 64, 64),   Color.White,            ColorOf(170, 90, 90)    )},
            { "90's Hacker",    (Color.Black,            Color.Lime,             ColorOf(0, 80, 0)       )},
            { "High Contrast",  (Color.Black,            Color.White,            Color.White             )}
        };

        private MainSettings()
        {
            FileName = "main";
            Load();
        }

        public override void ResetToDefaults()
        {
            Entries = new Dictionary<string, object>();
            Set(SHOW_BASIC, true);
            Set(FANCY_CORNERS, false);
            Set(COMPLETION_GLOW, false);
            Set(RAINBOW_MODE, false);
            Set(LAYOUT_DEBUG, false);
            Set(BACK_COLOR, Color.FromNonPremultiplied(240, 240, 240, 255));
            Set(TEXT_COLOR, Color.Black);
            Set(BORDER_COLOR, Color.FromNonPremultiplied(196, 196, 196, 255));
            base.ResetToDefaults();
        }
    }
}

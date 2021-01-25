using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class MainSettings : SettingsGroup
    {
        public static MainSettings Instance = new MainSettings();

        private const string SHOW_BASIC       = "show_basic_advancements";
        private const string ROUNDED_CORNERS  = "rounded_corners";
        private const string RAINBOW_MODE     = "rainbow_mode";
        private const string BACK_COLOR       = "main_back_color";
        private const string TEXT_COLOR       = "main_text_color";
        private const string BORDER_COLOR     = "main_border_color";

        public bool ShowBasic               { get => (bool)Entries[SHOW_BASIC];         set => Entries[SHOW_BASIC] = value; }
        public bool RenderRoundedCorners    { get => (bool)Entries[ROUNDED_CORNERS];    set => Entries[ROUNDED_CORNERS] = value; }
        public bool RainbowMode             { get => (bool)Entries[RAINBOW_MODE];       set => Entries[RAINBOW_MODE] = value; }
        public Color BackColor              { get => (Color)Entries[BACK_COLOR];        set => Entries[BACK_COLOR] = value; }
        public Color TextColor              { get => (Color)Entries[TEXT_COLOR];        set => Entries[TEXT_COLOR] = value; }
        public Color BorderColor            { get => (Color)Entries[BORDER_COLOR];      set => Entries[BORDER_COLOR] = value; }

        public static readonly Dictionary<string, (Color, Color, Color)> Themes = new Dictionary<string, (Color, Color, Color)>()
        {
            //establish theme presets
            { "Light Mode", (Color.FromNonPremultiplied(240, 240, 240, 255), Color.Black, Color.FromNonPremultiplied(196, 196, 196, 255)) },
            { "Dark Mode", (Color.FromNonPremultiplied(64, 64, 64, 255), Color.White, Color.FromNonPremultiplied(128, 128, 128, 255)) }
        };

        private MainSettings()
        {
            FileName = "main";
            Load();
        }

        public override void ResetToDefaults()
        {
            Entries = new Dictionary<string, object>()
            {
                { SHOW_BASIC,        true },
                { ROUNDED_CORNERS,   false },
                { RAINBOW_MODE,      false },
                { BACK_COLOR,   Color.FromNonPremultiplied(240, 240, 240, 255) },
                { TEXT_COLOR,   Color.Black },
                { BORDER_COLOR, Color.FromNonPremultiplied(196, 196, 196, 255) },
            };
        }
    }
}

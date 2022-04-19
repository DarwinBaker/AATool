using System.Collections.Generic;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace AATool.Configuration
{
    public partial class Config
    {
        [JsonObject]
        public class OverlayConfig : Config
        {
            [JsonProperty] public readonly Setting<bool> Enabled      = new (false);
            [JsonProperty] public readonly Setting<bool> ShowLabels   = new (true);
            [JsonProperty] public readonly Setting<bool> ShowCriteria = new (true);
            [JsonProperty] public readonly Setting<bool> ShowPickups  = new (true);
            [JsonProperty] public readonly Setting<bool> ShowIgt      = new (true);
            [JsonProperty] public readonly Setting<bool> RightToLeft  = new (false);
            [JsonProperty] public readonly Setting<bool> PickupsOpposite  = new (false);
            [JsonProperty] public readonly Setting<bool> ClarifyAmbiguous = new (true);

            [JsonProperty] public readonly Setting<string> FrameStyle = new ("Minecraft");

            [JsonProperty] public readonly Setting<int> Speed = new (2);
            [JsonProperty] public readonly Setting<int> Width = new (1920);

            [JsonProperty] public readonly Setting<Color> GreenScreen = new (new Color(0, 170, 0));
            [JsonProperty] public readonly Setting<Color> CustomTextColor = new (Hex("FFFFFF"));
            [JsonProperty] public readonly Setting<Color> CustomBackColor = new (Hex("FFFFFF"));
            [JsonProperty] public readonly Setting<Color> CustomBorderColor = new (Hex("FFFFFF"));

            [JsonProperty] public readonly Setting<WindowSnap> StartupArrangement = new (WindowSnap.TopLeft);
            [JsonProperty] public readonly Setting<Point> LastWindowPosition = new (Point.Zero);
            [JsonProperty] public readonly Setting<int> StartupDisplay = new (1);

            protected override string GetId() => "overlay";
            protected override string GetLegacyId() => "overlay";

            private static Color Hex(string hex) =>
                ColorHelper.TryGetHexColor(hex, out Color color) ? color : Color.White;

            public bool AppearanceChanged => this.FrameStyle.Changed
                || this.CustomTextColor.Changed
                || this.CustomBackColor.Changed
                || this.CustomBorderColor.Changed;

            public OverlayConfig()
            {
                this.RegisterSetting(this.Enabled);
                this.RegisterSetting(this.ShowLabels);
                this.RegisterSetting(this.ShowCriteria);
                this.RegisterSetting(this.ShowPickups);
                this.RegisterSetting(this.RightToLeft);
                this.RegisterSetting(this.PickupsOpposite);
                this.RegisterSetting(this.FrameStyle);
                this.RegisterSetting(this.Speed);
                this.RegisterSetting(this.Width);
                this.RegisterSetting(this.GreenScreen);
                this.RegisterSetting(this.CustomTextColor);
                this.RegisterSetting(this.CustomBackColor);
                this.RegisterSetting(this.CustomBorderColor);
                this.RegisterSetting(this.ShowIgt);
                this.RegisterSetting(this.ClarifyAmbiguous);
                this.RegisterSetting(this.StartupArrangement);
                this.RegisterSetting(this.StartupDisplay);
                this.RegisterSetting(this.LastWindowPosition);
            }

            protected override void ApplyLegacySetting(string key, object value)
            {
                ISetting setting = key switch {
                    "enabled"       => this.Enabled,
                    "show_labels"   => this.ShowLabels,
                    "show_criteria" => this.ShowCriteria,
                    "show_items"    => this.ShowPickups,
                    "show_igt"      => this.ShowIgt,
                    "right_to_left" => this.RightToLeft,
                    "speed"         => this.Speed,
                    "width"         => this.Width,
                    "back_color"    => this.GreenScreen,
                    "text_color"    => this.CustomTextColor,
                    _ => null
                };
                setting?.Set(value);
            }
        }
    }
}

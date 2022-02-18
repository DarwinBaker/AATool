using System.Collections.Generic;
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
            [JsonProperty] public readonly Setting<bool> RightToLeft  = new (true);
            [JsonProperty] public readonly Setting<bool> ClarifyAmbiguous = new (true);

            [JsonProperty] public readonly Setting<string> FrameStyle = new ("Minecraft");

            [JsonProperty] public readonly Setting<int> Speed = new (2);
            [JsonProperty] public readonly Setting<int> Width = new (1920);

            [JsonProperty] public readonly Setting<Color> BackColor = new (new Color(0, 170, 0));
            [JsonProperty] public readonly Setting<Color> TextColor = new (Color.White);

            protected override string GetId() => "overlay";
            protected override string GetLegacyId() => "overlay";

            public OverlayConfig()
            {
                this.RegisterSetting(this.Enabled);
                this.RegisterSetting(this.ShowLabels);
                this.RegisterSetting(this.ShowCriteria);
                this.RegisterSetting(this.ShowPickups);
                this.RegisterSetting(this.RightToLeft);
                this.RegisterSetting(this.FrameStyle);
                this.RegisterSetting(this.Speed);
                this.RegisterSetting(this.Width);
                this.RegisterSetting(this.BackColor);
                this.RegisterSetting(this.TextColor);
                this.RegisterSetting(this.ShowIgt);
                this.RegisterSetting(this.ClarifyAmbiguous);
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
                    "back_color"    => this.BackColor,
                    "text_color"    => this.TextColor,
                    _ => null
                };
                setting?.Set(value);
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
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
            [JsonProperty] public readonly Setting<bool> ShowLastRefresh = new (true);
            [JsonProperty] public readonly Setting<bool> RightToLeft  = new (false);
            [JsonProperty] public readonly Setting<bool> PickupsOpposite  = new (false);
            [JsonProperty] public readonly Setting<bool> LastRefreshOpposite  = new (false);
            [JsonProperty] public readonly Setting<bool> ClarifyAmbiguous = new (true);

            [JsonProperty] public readonly Setting<string> Position = new ("Minecraft");
            [JsonProperty] public readonly Setting<string> FrameStyle = new ("Minecraft");
            [JsonProperty] public readonly Setting<string> PrideFrameList = new (string.Empty);

            [JsonProperty] public readonly Setting<PinnedObjectiveSet> PinnedObjectiveList = new (new PinnedObjectiveSet());

            [JsonProperty] public readonly Setting<int> Speed = new (2);
            [JsonProperty] public readonly Setting<int> Width = new (1920);

            [JsonProperty] public readonly Setting<Color> GreenScreen = new (new Color(0, 170, 0));
            [JsonProperty] public readonly Setting<Color> CustomTextColor = new (Hex("FFFFFF"));
            [JsonProperty] public readonly Setting<Color> CustomBackColor = new (Hex("FFFFFF"));
            [JsonProperty] public readonly Setting<Color> CustomBorderColor = new (Hex("FFFFFF"));

            [JsonProperty] public readonly Setting<WindowSnap> StartupArrangement = new (WindowSnap.TopLeft);
            [JsonProperty] public readonly Setting<Point> LastWindowPosition = new (Point.Zero);
            [JsonProperty] public readonly Setting<int> StartupDisplay = new (1);

            [JsonIgnore]
            public bool AppearanceChanged => this.Enabled.Changed
                || this.FrameStyle.Changed
                || this.CustomBackColor.Changed
                || this.CustomBorderColor.Changed;

            [JsonIgnore]
            public bool ArrangementChanged => this.Enabled.Changed
                || this.RightToLeft.Changed
                || this.PickupsOpposite.Changed
                || this.ShowLastRefresh.Changed
                || this.LastRefreshOpposite.Changed;

            protected override string GetId() => "overlay";
            protected override string GetLegacyId() => "overlay";

            private static Color Hex(string hex) =>
                ColorHelper.TryGetHexColor(hex, out Color color) ? color : Color.White;

            [JsonIgnore] private string[] prideStyles;
            [JsonIgnore] private int styleIndex;

            public OverlayConfig()
            {
                this.RegisterSetting(this.Enabled);
                this.RegisterSetting(this.ShowLabels);
                this.RegisterSetting(this.ShowCriteria);
                this.RegisterSetting(this.ShowPickups);
                this.RegisterSetting(this.ShowLastRefresh);

                this.RegisterSetting(this.RightToLeft);
                this.RegisterSetting(this.PickupsOpposite);
                this.RegisterSetting(this.LastRefreshOpposite);
                this.RegisterSetting(this.FrameStyle);
                this.RegisterSetting(this.PrideFrameList);

                this.RegisterSetting(this.PinnedObjectiveList);

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

            protected override void ApplyDefaultValues()
            { 
                base.ApplyDefaultValues();
                this.PinnedObjectiveList.Set(new PinnedObjectiveSet());
            }

            public void SetPrideList(string csv)
            {
                this.PrideFrameList.Set(csv);
                this.prideStyles = csv.Split(',');
            }

            public string GetActiveFrameStyle(string currentStyle)
            {
                this.prideStyles ??= this.PrideFrameList.Value.Split(',');
                if (this.FrameStyle == "Multi-Pride" && this.prideStyles.Any())
                {
                    if (!string.IsNullOrEmpty(currentStyle) && this.prideStyles.Contains(currentStyle))
                        return currentStyle;

                    if (this.styleIndex >= this.prideStyles.Length)
                        this.styleIndex = 0;
                    string style = this.prideStyles[this.styleIndex];
                    this.styleIndex++;
                    return style;
                }
                return this.FrameStyle;
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

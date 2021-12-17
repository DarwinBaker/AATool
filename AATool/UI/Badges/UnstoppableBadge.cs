using AATool.UI.Controls;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class UnstoppableBadge : UIPicture
    {
        public int Scale { get; set; }
        private readonly UIGlowEffect glow;

        public UnstoppableBadge(int scale)
        {
            this.Scale = scale;
            this.FlexWidth = new(31 * (this.Scale / 2));
            this.FlexHeight = new(15 * (this.Scale / 2));
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
            this.Layer = Layer.Fore;

            this.glow = new UIGlowEffect() {
                Scale = this.Scale / 2,
                Brightness = 0,
            };
            this.AddControl(this.glow);
            this.SetTexture("badge_1k");
            if (this.Scale < 3)
            {
                this.glow.SetTexture("badge_1k_glow");
                this.glow.SkipToBrightness(0.75f);
            }
        }
    }
}

using AATool.UI.Controls;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class ImmortalBadge : UIPicture
    {
        public int Scale { get; set; }
        private readonly UIGlowEffect glow;

        public ImmortalBadge(int scale)
        {
            this.Scale = scale;
            this.FlexWidth = new(10 * this.Scale);
            this.FlexHeight = new(10 * this.Scale);
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
            this.Layer = Layer.Fore;

            this.glow = new UIGlowEffect() { 
                Scale = this.Scale / 2 ,
                Brightness = 0,
            };
            this.AddControl(this.glow);

            this.SetTexture("badge_hhh_up");
            if (this.Scale < 3)
            {
                this.glow.SetTexture("badge_hhh_glow");
                this.glow.SkipToBrightness(0.6f);
            }

        }

        protected override void UpdateThis(Time time)
        {
            if (time.TotalFrames % 5 is 0)
            {
                //wiggle hhh heart up and down
                if (Main.RNG.NextDouble() < 0.75)
                {
                    this.SetTexture("badge_hhh_up");
                    this.glow.MoveTo(new Point(this.glow.X, this.Top - this.Scale));
                }
                else
                {
                    this.SetTexture("badge_hhh_down");
                    this.glow.MoveTo(new Point(this.glow.X, this.Top - (this.Scale / 2)));
                }
            }
        }
    }
}

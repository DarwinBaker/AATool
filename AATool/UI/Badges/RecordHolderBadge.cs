using AATool.UI.Controls;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class RecordHolderBadge : UIPicture
    {
        public int Scale { get; set; }
        private readonly UIGlowEffect glow;

        public int Place { get; private set; }

        public RecordHolderBadge(int scale, int place)
        {
            this.Scale = scale;
            this.Place = place;
            this.FlexWidth = new(25 * (this.Scale / 2));
            this.FlexHeight = new(15 * (this.Scale / 2));
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
            this.Layer = Layer.Fore;

            this.glow = new UIGlowEffect() { 
                Scale = this.Scale / 2 ,
                Brightness = 0,
            };
            this.AddControl(this.glow);

            this.SetTexture($"badge_pb_{place}");
            if (this.Scale < 3)
            {
                this.glow.SetRotationSpeed(200f);
                this.glow.SetTexture($"badge_pb_{place}_glow");
                this.glow.SkipToBrightness(1f);
            }
        }

        public override void ResizeThis(Rectangle parent)
        {
            this.Margin = new Margin(-9, -0, -7, 0);
            this.Margin.Resize(parent.Size);
            this.FlexWidth.Resize(int.MaxValue);
            this.FlexHeight.Resize(int.MaxValue);

            //clamp size to min and max
            this.Width = this.FlexWidth;
            this.Height = this.FlexHeight;

            this.X = parent.Left + this.Margin.Left;
            this.Y = parent.Top + this.Margin.Top;

            this.Padding.Resize(this.Size);

            //calculate internal rectangle
            this.Inner = new Rectangle(
                this.X + this.Padding.Left,
                this.Y + this.Padding.Top,
                this.Width - this.Padding.Horizontal,
                this.Height - this.Padding.Vertical);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Graphics;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class HonorableBadge : UIControl
    {
        public int Scale { get; set; }
        private readonly UIGlowEffect glow;

        private const string Back = "badge_moleyg_1k";
        private const string Text = Back + "_text";

        public HonorableBadge(int scale)
        {
            this.Scale = scale;
            this.FlexWidth = new(34 * (this.Scale / 2));
            this.FlexHeight = new(15 * (this.Scale / 2));
            this.Margin = new Margin(-16, -0, -7, 0);
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
            this.Layer = Layer.Fore;

            this.glow = new UIGlowEffect() {
                Scale = this.Scale / 2,
                Brightness = 0,
            };
            this.AddControl(this.glow);
            if (this.Scale < 3)
            {
                this.glow.SetTexture("badge_couri_glow");
                this.glow.SetRotationSpeed(250f);
                this.glow.SkipToBrightness(0.75f);
            }
        }

        public override void ResizeThis(Rectangle parent)
        {
            this.Margin = new Margin(-18, -0, -7, 0);
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

        public override void DrawThis(Canvas canvas)
        {
            canvas.Draw(Back, this.Inner, Color.White, Layer.Fore);
            canvas.Draw(Text, this.Inner, Color.White, Layer.Fore);
        }
    }
}

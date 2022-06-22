using AATool.Graphics;
using AATool.UI.Controls;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class UnstoppableBadge : UIControl
    {
        public int Scale { get; set; }
        private readonly UIGlowEffect glow;

        private const string ThousandSeedsBack = "badge_couri_1k";
        private const string ThousandSeedsText = ThousandSeedsBack + "_text";
        private const string HundredHardcoreBack = "badge_couri_100hc";
        private const string HundredHardcoreText = HundredHardcoreBack + "_text";

        private static readonly SequenceTimer SwapTimer = new (60, 1, 0.75, 60, 1, 0.75);
        private static readonly bool ShowBoth = false;

        private static long LastFrameUpdated = -1;
        private static bool VariantsInitialized = false;

        public UnstoppableBadge(int scale)
        {
            this.Scale = scale;
            this.FlexWidth = new (34 * (this.Scale / 2));
            this.FlexHeight = new (15 * (this.Scale / 2));
            this.Margin = new Margin (-16, -0, -7, 0);
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
            this.Layer = Layer.Fore;

            if (!VariantsInitialized)
            {
                if (ShowBoth)
                    SwapTimer.Skip(2);
                VariantsInitialized = true;
            }

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

        protected override void UpdateThis(Time time)
        {
            if (ShowBoth && LastFrameUpdated != time.TotalFrames)
            {
                SwapTimer.Update(time);
                if (SwapTimer.IsExpired)
                    SwapTimer.Continue();
                LastFrameUpdated = time.TotalFrames;
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
            float ratio = (float)SwapTimer.Ratio;
            switch (SwapTimer.Index)
            {
                case 0:
                    canvas.Draw(ThousandSeedsBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(ThousandSeedsText, this.Inner, Color.White, Layer.Fore);
                    break;
                case 1:
                    canvas.Draw(HundredHardcoreBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(ThousandSeedsBack, this.Inner, ColorHelper.Fade(Color.White, (ratio / 2) + 0.5f), Layer.Fore);
                    canvas.Draw(ThousandSeedsText, this.Inner, Color.White * ratio, Layer.Fore);
                    break;
                case 2:
                    canvas.Draw(HundredHardcoreBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(ThousandSeedsBack, this.Inner, ColorHelper.Fade(Color.White, ratio / 2), Layer.Fore);
                    canvas.Draw(HundredHardcoreText, this.Inner, ColorHelper.Fade(Color.White, 1 - ratio), Layer.Fore);
                    break;
                case 3:
                    canvas.Draw(HundredHardcoreBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(HundredHardcoreText, this.Inner, Color.White, Layer.Fore);
                    break;
                case 4:
                    canvas.Draw(ThousandSeedsBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(HundredHardcoreBack, this.Inner, ColorHelper.Fade(Color.White, (ratio / 2) + 0.5f), Layer.Fore);
                    canvas.Draw(HundredHardcoreText, this.Inner, Color.White * ratio, Layer.Fore);
                    break;
                case 5:
                    canvas.Draw(ThousandSeedsBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(HundredHardcoreBack, this.Inner, ColorHelper.Fade(Color.White, ratio / 2), Layer.Fore);
                    canvas.Draw(ThousandSeedsText, this.Inner, ColorHelper.Fade(Color.White, 1 - ratio), Layer.Fore);
                    break;
            }
        }
    }
}

using AATool.Graphics;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class HalfHeartHardcoreBadge : Badge
    {
        private const string HeartUp = "badge_hhh_up";
        private const string HeartDown = "badge_hhh_down";
        private const string HeartGlow = "badge_hhh_glow";

        public override string GetListName => "Half Heart Hardcore";

        public HalfHeartHardcoreBadge()
        {
            this.FlexWidth = new (20);
            this.FlexHeight = new (20);
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
            this.Layer = Layer.Fore;
            this.Margin = new Margin(-13, -0, -10, 0);

            this.PopupBackColor = new (96, 27, 32);
            this.PopupBorderColor = new (213, 41, 52);
            this.PopupGlowColor = new (255, 80, 80, 128);
            this.Description.SetTextColor(new (255, 255, 255));

            this.BackTexture = HeartUp;
            this.Glow.SetTexture(HeartGlow);
            this.Glow.SkipToBrightness(0.6f);

            this.Description.SetText("Completed AA on Hardcore\nwith half a heart (4 times)");
        }

        protected override void UpdateThis(Time time)
        {
            base.UpdateThis(time);
            if (time.TotalFrames % 5 is 0)
            {
                //wiggle hhh heart up and down
                if (Main.RNG.NextDouble() < 0.75)
                {
                    this.BackTexture = HeartUp;
                    this.Glow.MoveTo(new Point(this.Glow.X, this.Top - 2));
                }
                else
                {
                    this.BackTexture = HeartDown;
                    this.Glow.MoveTo(new Point(this.Glow.X, this.Top - 1));
                }
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            this.DrawPopup(canvas);
            canvas.Draw(this.BackTexture, this.Inner, Color.White, Layer.Fore);
        }
    }
}

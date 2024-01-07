using AATool.Graphics;
using FontStashSharp;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class HundredHardcoreBadge : Badge
    {
        public override string GetListName => "100 Hardcore";

        int runs;
        string runString;

        static DynamicSpriteFont Font = null;

        public HundredHardcoreBadge(int runs) : base()
        {
            Font ??= FontSet.Get("minecraft", 12);

            this.runs = runs;

            if (this.runs >= 100)
            {
                this.BackTexture = "badge_hc_gold";
                this.Glow.SetTexture("badge_large_gold_glow");
                this.runString = runs.ToString();
            }
            else if (this.runs >= 50)
            {
                this.BackTexture = "badge_hc_silver";
                this.Glow.SetTexture("badge_large_silver_glow");
                this.runString = $" {runs}";
            }
            else 
            {
                this.BackTexture = "badge_hc_bronze";
                this.Glow.SetTexture("badge_rank_2_glow");
                this.runString = $" {runs}";
            }

            this.TextTexture = "badge_hc_icon";

            this.FlexWidth = new (34);
            this.FlexHeight = new (15);
            this.Margin = new Margin (-10, -0, -7, 0);

            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;

            this.PopupBackColor = new (96, 27, 32);
            this.PopupBorderColor = new (213, 41, 52);
            this.PopupGlowColor = new (255, 80, 80, 128);
            this.Description.SetTextColor(new(255, 255, 255));

            this.Glow.SkipToBrightness(0.75f);

            this.Description.SetText($"Completed {this.runs} deathless\nHC no-reset Any% in a row");
        }

        public override void DrawThis(Canvas canvas)
        {
            base.DrawThis(canvas);

            canvas.DrawString(Font, 
                this.runString, 
                new Vector2(this.Left + 4, this.Top + 1), 
                Color.White, 
                Layer.Fore);
        }
    }
}
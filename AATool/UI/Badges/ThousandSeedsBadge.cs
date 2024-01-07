using AATool.Data;
using AATool.Graphics;
using FontStashSharp;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class NoResetsBadge : Badge
    {
        private string text;
        private int runs;

        public override string GetListName => $"{this.runs:N0} Seeds";

        static DynamicSpriteFont Font = null;

        public NoResetsBadge(string player, int runs) : base()
        {
            Font ??= FontSet.Get("minecraft", 12);

            this.runs = runs;
            this.text = this.runs.ToString("N0");

            if (this.runs >= 100000)
            {
                this.BackTexture = $"badge_100k";
                this.FlexWidth = new(46);
                this.Margin = new Margin(-22, -0, -7, 0);
            }
            else if (this.runs >= 10000)
            {
                this.BackTexture = $"badge_10k";
                this.FlexWidth = new(40);
                this.Margin = new Margin(-16, -0, -7, 0);
            }
            else
            {
                this.BackTexture = $"badge_1k";
                this.FlexWidth = new(34);
                this.Margin = new Margin(-10, -0, -7, 0);
            }

            /*
            if (player is Credits.CouriwayName)
            {
                this.PopupBackColor = new (80, 40, 128);
                this.PopupBorderColor = new (131, 54, 182);
                this.PopupGlowColor = new (183, 120, 196, 180);
            }
            else if (player is Credits.MoleyGName)
            {
                this.PopupBackColor = new (8, 68, 160);
                this.PopupBorderColor = new (5, 112, 191);
                this.PopupGlowColor = new (120, 120, 196, 180);
            }
            */

            this.PopupBackColor = new(80, 40, 128);
            this.PopupBorderColor = new(131, 54, 182);
            this.PopupGlowColor = new(183, 120, 196, 180);

            
            this.FlexHeight = new (15);
            
     
            this.Description.SetText($"Completed {runs:N0} Any%\n No-Reset Runs");
            this.Description.SetTextColor(Color.White);

            this.Glow.SetTexture("badge_large_gold_glow");
            this.Glow.SkipToBrightness(0.75f);
        }

        public override void DrawThis(Canvas canvas)
        {
            base.DrawThis(canvas);

            canvas.DrawString(Font,
                this.text,
                new Vector2(this.Left + 4, this.Top + 1),
                Color.White,
                Layer.Fore);
        }
    }
}
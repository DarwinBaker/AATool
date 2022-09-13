using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class ThousandSeedsBadge : Badge
    {
        public ThousandSeedsBadge(string player) : base()
        {
            this.BackTexture = $"badge_1k_{player}";
            this.TextTexture = "badge_1k_text";

            if (player is CouriwayName)
            {
                this.PopupBackColor = new (80, 40, 128);
                this.PopupBorderColor = new (131, 54, 182);
                this.PopupGlowColor = new (183, 120, 196, 180);
            }
            else if (player is MoleyGName)
            {
                this.PopupBackColor = new (8, 68, 160);
                this.PopupBorderColor = new (5, 112, 191);
                this.PopupGlowColor = new (120, 120, 196, 180);
            }

            this.FlexWidth = new (34);
            this.FlexHeight = new (15);
            this.Margin = new Margin (-10, -0, -7, 0);
     
            this.Description.SetText("Completed Any% on 1,000\nseeds without resetting");
            this.Description.SetTextColor(Color.White);

            this.Glow.SetTexture("badge_large_gold_glow");
            this.Glow.SkipToBrightness(0.75f);
        }
    }
}
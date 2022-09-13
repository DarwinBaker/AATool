namespace AATool.UI.Badges
{
    class HundredHardcoreBadge : Badge
    {
        public HundredHardcoreBadge() : base()
        {
            this.BackTexture = "badge_100hc";
            this.TextTexture = "badge_100hc_text";

            this.FlexWidth = new (34);
            this.FlexHeight = new (15);
            this.Margin = new Margin (-10, -0, -7, 0);

            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;

            this.PopupBackColor = new (96, 27, 32);
            this.PopupBorderColor = new (213, 41, 52);
            this.PopupGlowColor = new (255, 80, 80, 128);
            this.Description.SetTextColor(new(255, 255, 255));

            this.Glow.SetTexture("badge_large_gold_glow");
            this.Glow.SkipToBrightness(0.75f);

            this.Description.SetText("Complete 100 deathless\nHC no-reset Any% in a row");
        }
    }
}
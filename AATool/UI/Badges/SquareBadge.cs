namespace AATool.UI.Badges
{
    abstract class SquareBadge : Badge
    {
        public SquareBadge() : base()
        {
            this.FlexWidth = new (22);
            this.FlexHeight = new (22);
            this.Margin = new Margin(-10, -0, -10, 0);

            this.Glow.SetTexture("badge_rank_2_glow");
            this.Glow.SkipToBrightness(0.9f);
            this.Glow.Scale = 1.2f;
        }
    }
}
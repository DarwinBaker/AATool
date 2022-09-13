namespace AATool.UI.Badges
{
    class VipBadge : SquareBadge
    {
        public VipBadge(string description) : base()
        {
            this.BackTexture = "badge_vip";
            this.Description.SetText(description);
        }
    }
}
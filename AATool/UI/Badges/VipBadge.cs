namespace AATool.UI.Badges
{
    public class VipBadge : SquareBadge
    {
        public override string GetListName => "VIP";

        public VipBadge(string description) : base()
        {
            this.BackTexture = "badge_vip";
            this.Description.SetText(description);
        }
    }
}
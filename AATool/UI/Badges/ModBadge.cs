namespace AATool.UI.Badges
{
    class ModBadge : SquareBadge
    {
        public override string GetListName => "Moderator";

        public ModBadge(string description) : base()
        {
            this.BackTexture = "badge_mod";
            this.Description.SetText(description);
        }
    }
}
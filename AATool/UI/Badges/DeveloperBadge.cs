using AATool.Graphics;

namespace AATool.UI.Badges
{
    class DeveloperBadge : SquareBadge
    {
        public override string GetListName => "Developer";

        public DeveloperBadge() : base()
        {
            this.BackTexture = "badge_dev";
            this.Description.SetText("The Developer of AATool");
        }

        protected override void UpdateThis(Time time) 
        { 
            base.UpdateThis(time);

            if (this.Hovering)
                this.Description.SetTextColor(Canvas.RainbowFast);
        }
    }
}
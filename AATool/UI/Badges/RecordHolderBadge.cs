using AATool.UI.Controls;

namespace AATool.UI.Badges
{
    class RecordHolderBadge : UIPicture
    {
        public int Scale { get; set; }
        private readonly UIGlowEffect glow;

        public RecordHolderBadge(int scale, int place)
        {
            this.Scale = scale;
            this.FlexWidth = new(25 * (this.Scale / 2));
            this.FlexHeight = new(15 * (this.Scale / 2));
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
            this.Layer = Layer.Fore;

            this.glow = new UIGlowEffect() { 
                Scale = this.Scale / 2 ,
                Brightness = 0,
            };
            this.AddControl(this.glow);

            this.SetTexture($"badge_pb_{place}");
            if (this.Scale < 3)
            {
                this.glow.SetTexture($"badge_pb_{place}_glow");
                this.glow.SkipToBrightness(1f);
            }
        }
    }
}

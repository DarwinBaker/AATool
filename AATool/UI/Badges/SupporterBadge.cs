using AATool.Data;
using AATool.Graphics;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    public class SupporterBadge : SquareBadge
    {
        public const string ListNameNetherite = "Supporter (Netherite)";
        public const string ListNameDiamond = "Supporter (Diamond)";
        public const string ListNameGold = "Supporter (Gold)";

        private static readonly Color GlintColor = new (200, 200, 200, 200);
        private static readonly Color PopupGlintColor = new (140, 140, 140, 140);
        private static readonly Color GoldGlowColor = new (230, 200, 140, 140);
        private static readonly Color DiamondGlowColor = new (80, 140, 190, 205);
        private static readonly Color NetheriteGlowColor = new (215, 150, 255, 110);

        public readonly string Role;

        public SupporterBadge(string role) : base()
        {
            this.Margin = new Margin(-9, -0, -9, 0);
            this.Padding = new Margin(1, 1, 1, 1);
            
            this.Role = role;
            this.BackTexture = $"badge_{this.Role}";
            if (this.Role is Credits.NetheriteTier)
                this.Description.SetText("- AATool Supporter -\nNetherite Tier");
            else if(this.Role is Credits.DiamondTier)
                this.Description.SetText("- AATool Supporter -\nDiamond Tier");
            else if (this.Role is Credits.GoldTier)
                this.Description.SetText("- AATool Supporter -\nGold Tier");
            
            this.Glow.SkipToBrightness(0.75f);
            if (this.Role is Credits.GoldTier)
                this.Glow.SetTexture($"badge_rank_1_glow");
            else if (this.Role is Credits.DiamondTier)
                this.Glow.SetTexture($"badge_rank_diamond_glow");
            else if (this.Role is Credits.NetheriteTier)
                this.Glow.SetTexture($"badge_rank_netherite_glow");
        }

        public override void DrawThis(Canvas canvas)
        {
            this.DrawPopup(canvas);
            canvas.Draw(this.BackTexture, this.Bounds, Color.White, Layer.Fore);
            canvas.Draw(this.TextTexture, this.Bounds, Color.White, Layer.Fore);

            if (this.Role is Credits.NetheriteTier)
            {
                canvas.Draw("glint", this.Inner, GlintColor, Layer.Fore);
                canvas.Draw("badge_supporter_netherite_icon", this.Bounds, Color.White, Layer.Fore);
            }
            else if (this.Role is Credits.DiamondTier)
            {
                canvas.Draw("glint", this.Inner, GlintColor, Layer.Fore);
                canvas.Draw("badge_supporter_diamond_icon", this.Bounds, Color.White, Layer.Fore);
            }
        }

        protected override void DrawPopup(Canvas canvas)
        {
            if (!this.Hovering || !HoverTimer.IsExpired || HoverTimer.TimeElapsed <= 0)
                return;

            if (this.Role is Credits.NetheriteTier)
            {
                canvas.Draw(PopupGlow, this.Description.Bounds, NetheriteGlowColor, Layer.Fore);
                canvas.Draw("popup_badge_netherite", this.Description.Inner, Color.Lavender, Layer.Fore);
                canvas.Draw("glint", this.Description.Inner, PopupGlintColor, Layer.Fore);
                canvas.Draw("popup_badge_netherite_gild", this.Description.Inner, Color.White, Layer.Fore);
                
            }
            else if (this.Role is Credits.DiamondTier)
            {
                canvas.Draw(PopupGlow, this.Description.Bounds, DiamondGlowColor, Layer.Fore);
                canvas.Draw("popup_badge_diamond", this.Description.Inner, Color.White, Layer.Fore);
                canvas.Draw("glint", this.Description.Inner, GlintColor, Layer.Fore);
            }
            else if (this.Role is Credits.GoldTier)
            {
                canvas.Draw(PopupGlow, this.Description.Bounds, GoldGlowColor, Layer.Fore);
                canvas.Draw("popup_badge_gold", this.Description.Inner, Color.White, Layer.Fore);
            }
            else
            {
                base.DrawPopup(canvas);
            }
        }
    }
}

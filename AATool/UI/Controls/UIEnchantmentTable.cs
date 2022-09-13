using System;
using AATool.Configuration;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIEnchantmentTable : UIPicture
    {
        private const string Reading = "enchantment_table_reading";
        private const string Opening = "enchantment_table_opening";
        private const string Closing = "enchantment_table_closing";
        private const string Closed  = "enchantment_table_closed";
        private const string TableGlow = "enchantment_table_glow";
        private const string Anchor = "respawn_anchor_top_on";
        private const string AnchorGlow = "respawn_anchor_glow";

        private readonly AnimatedSprite open;
        private readonly AnimatedSprite close;

        private UIPicture portal;

        private float glowRotation;
        private float glowWidth;
        
        public UIEnchantmentTable()
        {
            this.SetLayer(Layer.Fore);
            if (Tracker.IsWorking)
            {
                this.SetTexture(Reading);
                this.glowWidth = 1;
            }
            else
            {
                this.SetTexture(Closed);
            }

            if (SpriteSheet.TryGet(Opening, out Sprite sprite) && sprite is AnimatedSprite openAnimation)
                this.open = openAnimation;
            if (SpriteSheet.TryGet(Closing, out sprite) && sprite is AnimatedSprite closeAnimation)
                this.close = closeAnimation;

            this.portal = new UIPicture() {
                FlexWidth = new Size(32),
                FlexHeight = new Size(32),
                Margin = new Margin(0, 0, 3, 0),
                VerticalAlign = VerticalAlign.Top,
            };
            this.AddControl(this.portal);
        }

        public override void InitializeRecursive(UIScreen screen) 
        {
            this.portal.SetTexture(Anchor);
            base.InitializeRecursive(screen); 
        }

        private bool IsFirstFrameOf(AnimatedSprite sprite) =>
            sprite.CurrentFrame is 0;

        private bool IsLastFrameOf(AnimatedSprite sprite) =>
            sprite.CurrentFrame == sprite.Frames - 1;
        
        public void UpdateState(bool isReadingSave)
        {
            if (string.IsNullOrEmpty(this.Texture))
            {
                this.SetTexture(Closed);
            }
            else if (isReadingSave)
            {
                switch (this.Texture)
                {
                    case Closed:
                        if (this.IsFirstFrameOf(this.open))
                            this.SetTexture(Opening);
                        break;
                    case Anchor:
                        this.SetTexture(Closed);
                        break;
                    case Closing:
                        if (this.IsLastFrameOf(this.close))
                            this.SetTexture(Closed);
                        break;
                    case Opening:
                        if (this.IsLastFrameOf(this.open))
                            this.SetTexture(Reading);
                        break;
                }
            }
            else
            {
                if (this.Texture is Reading && this.IsFirstFrameOf(this.close))
                    this.SetTexture(Closing);
                else if (this.Texture is Closing && this.IsLastFrameOf(this.close))
                    this.SetTexture(Closed);
                else if (this.Texture is Opening && this.IsLastFrameOf(this.open))
                    this.SetTexture(Reading);
            }
        }

        protected override void UpdateThis(Time time)
        {
            if (Peer.IsConnected || Config.Tracking.UseSftp || (Client.TryGet(out Client client) && client.LostConnection))
                this.portal.Expand();
            else
                this.portal.Collapse();

            //handle glow effect rotation
            if (this.glowWidth is 1)
                this.glowRotation += (float)time.Delta * 0.25f;
            else
                this.glowRotation = 0;

            this.glowWidth = this.Texture is Reading or Opening
                ? (float)Math.Min(this.glowWidth + (time.Delta * 1.2), 1)
                : (float)Math.Max(this.glowWidth - (time.Delta * 1.3), 0);
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.portal.IsCollapsed)
                base.DrawThis(canvas);
            
            Vector2 center = this.portal.IsCollapsed ? new (this.Center.X, this.Center.Y - 4) : new (this.Center.X + 1, this.Center.Y + 1);
            Color tint = this.portal.IsCollapsed ? Color.White : Color.Violet;
            Vector2 scale = this.portal.IsCollapsed ? new Vector2(this.glowWidth, 1) : new Vector2(1, 1);
            string texture = this.portal.IsCollapsed ? TableGlow : AnchorGlow;
            canvas.Draw(texture, center, this.glowRotation, scale, tint, Layer.Glow);
        }
    }
}

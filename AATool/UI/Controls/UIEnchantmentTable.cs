using System;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace AATool.UI.Controls
{
    class UIEnchantmentTable : UIPicture
    {
        private const string TEXTURE_READING = "enchantment_table_reading";
        private const string TEXTURE_OPENING = "enchantment_table_opening";
        private const string TEXTURE_CLOSING = "enchantment_table_closing";
        private const string TEXTURE_CLOSED  = "enchantment_table_closed";
        private const string TEXTURE_GLOW    = "enchantment_table_glow";

        private const string TEXTURE_PORTAL = "respawn_anchor_top_on";

        private readonly Sprite open;
        private readonly Sprite close;

        private UIPicture portal;

        private float glowRotation;
        private float glowWidth;
        
        public UIEnchantmentTable()
        {
            this.SetLayer(Layer.Fore);
            this.SetTexture(TEXTURE_CLOSED);
            SpriteSheet.Sprites.TryGetValue(TEXTURE_OPENING, out this.open);
            SpriteSheet.Sprites.TryGetValue(TEXTURE_CLOSING, out this.close);
        }

        public override void InitializeRecursive(UIScreen screen) 
        {
            this.portal = new UIPicture() {
                FlexWidth = new Size(32),
                FlexHeight = new Size(32),
                HorizontalAlign = HorizontalAlign.Left,
                VerticalAlign = VerticalAlign.Top,
                Margin = new Margin(3, 0, 3, 0)
            };
            this.portal.SetTexture(TEXTURE_PORTAL);
            this.AddControl(this.portal);
            base.InitializeRecursive(screen); 
        }

        private bool IsFirstFrameOf(Sprite sprite) => 
            SpriteSheet.GetFrameIndex(sprite.Frames) is 0;
        private bool IsLastFrameOf(Sprite sprite) => 
            SpriteSheet.GetFrameIndex(sprite.Frames) == sprite.Frames - 1;
        
        public void UpdateState(bool isReadingSave)
        {
            if (string.IsNullOrEmpty(this.Texture))
            {
                this.SetTexture(TEXTURE_CLOSED);
            }
            else if (isReadingSave)
            {
                switch (this.Texture)
                {
                    case TEXTURE_CLOSED:
                        if (this.IsFirstFrameOf(this.open))
                            this.SetTexture(TEXTURE_OPENING);
                        break;
                    case TEXTURE_PORTAL:
                        this.SetTexture(TEXTURE_CLOSED);
                        break;
                    case TEXTURE_CLOSING:
                        if (this.IsLastFrameOf(this.close))
                            this.SetTexture(TEXTURE_CLOSED);
                        break;
                    case TEXTURE_OPENING:
                        if (this.IsLastFrameOf(this.open))
                            this.SetTexture(TEXTURE_READING);
                        break;
                }
            }
            else
            {
                if (this.Texture is TEXTURE_READING && this.IsFirstFrameOf(this.close))
                    this.SetTexture(TEXTURE_CLOSING);
                else if (this.Texture is TEXTURE_CLOSING && this.IsLastFrameOf(this.close))
                    this.SetTexture(TEXTURE_CLOSED);
                else if (this.Texture is TEXTURE_OPENING && this.IsLastFrameOf(this.open))
                    this.SetTexture(TEXTURE_READING);
            }
        }

        protected override void UpdateThis(Time time)
        {
            if (Peer.IsConnected)
                this.portal.Expand();
            else
                this.portal.Collapse();

            //handle glow effect rotation
            if (this.glowWidth is 1)
                this.glowRotation += (float)time.Delta * 0.25f;
            else
                this.glowRotation = 0;

            this.glowWidth = this.Texture is TEXTURE_READING or TEXTURE_OPENING
                ? (float)Math.Min(this.glowWidth + (time.Delta * 2.2), 1)
                : (float)Math.Max(this.glowWidth - (time.Delta * 2), 0);
        }

        public override void DrawThis(Display display)
        {
            if (!Peer.IsConnected)
            {
                base.DrawThis(display);
            }
            
            Vector2 center = this.portal.IsCollapsed ? new (this.Center.X, this.Center.Y - 4) : new (this.Center.X + 1, this.Center.Y + 1);
            Color tint     = this.portal.IsCollapsed ? Color.White : Color.Violet;
            Vector2 scale  = this.portal.IsCollapsed ? new Vector2(this.glowWidth, 1) : new Vector2(1, 1);
            string texture = this.portal.IsCollapsed ? TEXTURE_GLOW : "respawn_anchor_glow";
            display.Draw(texture, center, this.glowRotation, scale, tint, Layer.Glow);
        }
    }
}

using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIPicture : UIControl
    {
        public string Texture { get; private set; }
        public Color Tint     { get; private set; }
        public float Rotation { get; private set; }

        public void SetTexture(string texture)
        {
            if (this.Texture != texture && this.Layer is Layer.Main && this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
            this.Texture = texture;

            if (SpriteSheet.IsAnimated(this.Texture ?? string.Empty))
                this.Layer = Layer.Fore;
        }

        public void SetTint(Color tint)
        {
            if (this.Tint != tint && this.Layer is Layer.Main && this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
            this.Tint = tint;
        }

        public void SetRotation(float rotation)
        {
            if (this.Rotation != rotation && this.Layer is Layer.Main && this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
            this.Rotation = rotation;
        }

        public UIPicture()
        {
            this.Tint = Color.White;
        }

        public override void DrawThis(Display display)
        {
            if (this.SkipDraw)
                return;

            if (this.Rotation is 0)
                display.Draw(this.Texture, this.Content, this.Tint, this.Layer);
            else
                display.Draw(this.Texture, this.Content.Center.ToVector2(), this.Rotation, 1, this.Tint, this.Layer);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.Texture = ParseAttribute(node, "texture", this.Texture ?? string.Empty);
            this.Tint = ParseAttribute(node, "tint", this.Tint);
        }
    }
}

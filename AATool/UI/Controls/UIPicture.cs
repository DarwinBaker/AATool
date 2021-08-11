using AATool.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIPicture : UIControl
    {
        public string Texture { get; private set; }
        public Color Tint     { get; private set; }
        public Layer Layer    { get; private set; }
        public float Rotation { get; private set; }

        public void SetTexture(string texture)  => this.Texture = texture;
        public void SetTint(Color tint)         => this.Tint = tint;
        public void SetLayer(Layer layer)       => this.Layer = layer;
        public void SetRotation(float rotation) => this.Rotation = rotation;

        public UIPicture()
        {
            this.Tint = Color.White;
        }

        public override void DrawThis(Display display)
        {
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
            this.SetLayer(ParseAttribute(node, "layer", this.Layer));
        }
    }
}

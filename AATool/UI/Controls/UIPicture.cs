using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIPicture : UIControl
    {
        public string Texture { get; protected set; }
        public Color Tint     { get; protected set; }

        public void SetTexture(string texture)  => Texture = texture;
        public void SetTint(Color tint)         => Tint = tint;

        public UIPicture()
        {
            Tint = Color.White;
        }

        public override void DrawThis(Display display)
        {
            display.Draw(Texture, ContentRectangle, Tint);
        }
    }
}

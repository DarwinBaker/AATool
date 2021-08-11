using System.Xml;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIGlowEffect : UIPicture
    {
        public float Brightness { get; private set; }

        private float displayBrightness;

        private bool isMainWindow;

        public void LerpToBrightness(float brightness) => this.Brightness = brightness;

        public void SkipToBrightness(float brightness)
        { 
             this.Brightness = brightness;
             this.displayBrightness = this.Brightness;
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.isMainWindow = screen is UIMainScreen;
            this.displayBrightness = this.Brightness;
        }

        protected override void UpdateThis(Time time)
        {
            float offset = this.isMainWindow
                ? (this.X * 100) + (this.Y * 100)
                : 0;
            this.SetRotation((float)(offset + (time.TotalSeconds % (MathHelper.TwoPi * 8f) / 8f)));

            this.displayBrightness = MathHelper.Lerp(this.displayBrightness, this.Brightness, (float)(10 * time.Delta));
        }

        public override void DrawThis(Display display)
        {
            display.Draw(this.Texture, this.Content.Center.ToVector2(), this.Rotation, 1, this.Tint * this.displayBrightness, Layer.Glow);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.LerpToBrightness(ParseAttribute(node, "brightness", 1));
        }
    }
}

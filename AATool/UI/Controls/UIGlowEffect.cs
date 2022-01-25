using System.Xml;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIGlowEffect : UIPicture
    {
        public float Brightness { get; set; }
        public float Scale { get; set; }

        private float displayBrightness;

        private bool isMainWindow;

        public UIGlowEffect()
        {
            this.Layer = Layer.Glow;
        }

        public void LerpToBrightness(float brightness) => this.Brightness = MathHelper.Clamp(brightness, 0, 1);

        public void SkipToBrightness(float brightness)
        {
            this.Brightness = brightness;
            this.displayBrightness = this.Brightness;
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.Layer = Layer.Glow;
            this.isMainWindow = screen is UIMainScreen;
            this.displayBrightness = this.Brightness;
        }

        protected override void UpdateThis(Time time)
        {
            float offset = this.isMainWindow
                ? (this.X * 100) + (this.Y * 100)
                : 0;

            this.SetRotation((float)(offset + (time.TotalFrames / 400f)));
            this.displayBrightness = MathHelper.Lerp(this.displayBrightness, this.Brightness, (float)(10 * time.Delta));
        }

        public override void DrawThis(Canvas canvas)
        {
            canvas.Draw(this.Texture, this.Inner.Center.ToVector2(), this.Rotation, this.Scale, this.Tint * this.displayBrightness, Layer.Glow);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.LerpToBrightness(Attribute(node, "brightness", 1f));
            this.Scale = Attribute(node, "scale", 1f);
        }
    }
}

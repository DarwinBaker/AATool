using System;
using AATool.Graphics;
using AATool.Settings;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool
{
    public class Display
    {
        public Color RainbowColor { get; private set; }

        private SpriteBatch batch;

        public Display(GraphicsDeviceManager manager)
        {
            batch = new SpriteBatch(manager.GraphicsDevice);
            
            //configure graphics parameters
            manager.GraphicsProfile = GraphicsProfile.Reach;
            manager.SynchronizeWithVerticalRetrace = true;
            manager.HardwareModeSwitch = false;
            manager.ApplyChanges();
        }

        public void Begin(BlendState blend = null) => batch.Begin(SpriteSortMode.Immediate, blend ?? BlendState.NonPremultiplied, SamplerState.PointClamp);
        public void End()                          => batch.End();

        public void Update(Time time)
        {
            UpdateRainbowColor(time);
        }

        public void Draw(string texture, Rectangle rectangle, Color? tint = null, int frameCount = 1)
        {
            if (string.IsNullOrWhiteSpace(texture))
                return;

            //if texture id not found, check for resolution specific variant of it
            var source = SpriteSheet.RectangleOf(texture);
            if (source.IsEmpty)
                source = SpriteSheet.RectangleOf(texture + SpriteSheet.RESOLUTION_PREFIX + rectangle.Width);

            if (frameCount == 1)
                batch.Draw(SpriteSheet.Atlas, rectangle, source, tint ?? Color.White);
            else
            {
                //sprite is animated; calculate sub-rectangle for current frame
                batch.Draw(SpriteSheet.Atlas, rectangle, source, tint ?? Color.White);
            }
        }

        public void DrawRectangle(Rectangle rectangle, Color color, Color? borderColor = null, int border = 0)
        {
            borderColor ??= color;
            if (border > 0 && borderColor != color)
            {
                //draw rectangle with border
                batch.Draw(SpriteSheet.Atlas, rectangle, SpriteSheet.RectangleOf("pixel"), borderColor.Value);
                var inner = new Rectangle(rectangle.X + border, rectangle.Y + border, rectangle.Width - border * 2, rectangle.Height - border * 2);
                batch.Draw(SpriteSheet.Atlas, inner, SpriteSheet.RectangleOf("pixel"), color);

                //draw rounded corners
                if (MainSettings.Instance.RenderFancyCorners)
                {
                    batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Left + 1, rectangle.Top + 1, 1, 1), SpriteSheet.RectangleOf("pixel"), borderColor.Value);
                    batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Right - 2, rectangle.Top + 1, 1, 1), SpriteSheet.RectangleOf("pixel"), borderColor.Value);
                    batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Right - 2, rectangle.Bottom - 2, 1, 1), SpriteSheet.RectangleOf("pixel"), borderColor.Value);
                    batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Left + 1, rectangle.Bottom - 2, 1, 1), SpriteSheet.RectangleOf("pixel"), borderColor.Value);
                }
            }
            else
                batch.Draw(SpriteSheet.Atlas, rectangle, SpriteSheet.RectangleOf("pixel"), color);
        }

        public void DrawLine(Vector2 a, Vector2 b, int thickness, Color? tint = null)
        {
            //draw a line between two points
            float dist = Vector2.Distance(a, b);
            float angle = (float)Math.Acos(Vector2.Dot(Vector2.Normalize(a - b), -Vector2.UnitX));
            if (a.Y > b.Y)
                angle = MathHelper.TwoPi - angle;

            batch.Draw(SpriteSheet.Atlas, new Rectangle((int)a.X, (int)a.Y, (int)dist, thickness), SpriteSheet.RectangleOf("pixel"), tint ?? Color.White, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawString(DynamicSpriteFont font, string text, Vector2 location, Color color)
        {
            batch.DrawString(font, text, location, color);
        }

        private void UpdateRainbowColor(Time time)
        {
            //fade rainbow to next color
            float hue = time.TotalFrames % 360;
            int primary = Convert.ToInt32(Math.Floor(hue / 60)) % 6;

            double f = hue / 60 - Math.Floor(hue / 60);
            double sat = 0.25;
            double value = 255;

            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - sat));
            int q = Convert.ToInt32(value * (1 - f * sat));
            int t = Convert.ToInt32(value * (1 - (1 - f) * sat));

            RainbowColor = primary switch
            {
                0 => new Color(v, t, p, 255),
                1 => new Color(q, v, p, 255),
                2 => new Color(p, v, t, 255),
                3 => new Color(p, q, v, 255),
                4 => new Color(t, p, v, 255),
                _ => new Color(v, p, q, 255)
            };
        }
    }
}

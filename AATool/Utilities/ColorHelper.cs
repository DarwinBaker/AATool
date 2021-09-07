using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;

namespace AATool.Utilities
{
    public static class ColorHelper
    {
        private const NumberStyles HEX_STLYE = NumberStyles.AllowHexSpecifier;

        public static Color ToXNA(System.Drawing.Color color) => 
            new (color.R, color.G, color.B, color.A);
        public static System.Drawing.Color ToDrawing(Color color) => 
            System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        
        public static Color FromRGB(int r, int g, int b)          => new (r, g, b);
        public static Color FromRGBA(int r, int g, int b, int a)  => new (r, g, b, a);

        public static Color Fade(Color color, float opacity) => new (color.R, color.G, color.B, opacity);
        public static Color ColorFromHSV(double hue, double sat, double val)
        {
            int primary = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = (hue / 60) - Math.Floor(hue / 60);
            val *= 255;

            int v = Convert.ToInt32(val);
            int p = Convert.ToInt32(val * (1 - sat));
            int q = Convert.ToInt32(val * (1 - (f * sat)));
            int t = Convert.ToInt32(val * (1 - ((1 - f) * sat)));

            return primary switch {
                0 => new Color(v, t, p, 255),
                1 => new Color(q, v, p, 255),
                2 => new Color(p, v, t, 255),
                3 => new Color(p, q, v, 255),
                4 => new Color(t, p, v, 255),
                _ => new Color(v, p, q, 255)
            };
        }

        public static string ToHexString(Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        public static string ToHexString(System.Drawing.Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        public static bool TryGetHexColor(string hex, out Color color)
        {
            try
            {
                //Remove # if present
                if (hex.IndexOf('#') is 0)
                    hex = hex.Remove(0, 1);

                int r = 0, g = 0, b = 0;
                //support both long and short hex codes
                if (hex.Length == 6)
                {
                    r = int.Parse(hex.Substring(0, 2), HEX_STLYE);
                    g = int.Parse(hex.Substring(2, 2), HEX_STLYE);
                    b = int.Parse(hex.Substring(4, 2), HEX_STLYE);
                }
                else if (hex.Length == 3)
                {
                    r = int.Parse(hex[0].ToString() + hex[0], HEX_STLYE);
                    g = int.Parse(hex[1].ToString() + hex[1], HEX_STLYE);
                    b = int.Parse(hex[2].ToString() + hex[2], HEX_STLYE);
                }
                color = FromRGB(r, g, b);
                return true;
            }
            catch 
            {
                color = Color.Transparent;
                return false;
            }
        }

        public static Color Blend(Color fore, Color back, double amount)
        {
            byte r = (byte)((fore.R * amount) + (back.R * (1 - amount)));
            byte g = (byte)((fore.G * amount) + (back.G * (1 - amount)));
            byte b = (byte)((fore.B * amount) + (back.B * (1 - amount)));
            return Color.FromNonPremultiplied(r, g, b, 255);
        }

        public static Color Amplify(Color color, float amount)
        {
            var drawing = ToDrawing(color);
            float h = drawing.GetHue();
            float s = drawing.GetSaturation();
            float l = drawing.GetBrightness();
            return ColorFromHSV(h, s * amount, l);
        }

        public static Color GetAccent(Texture2D texture)
        {
            float third = 256f / 3;
            float r = 0;
            float g = 0;
            float b = 0;
            float total = 0;
            var colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            for (int i = 0; i < colors.Length; i++)
            {
                Color pixel = colors[i];
                if (pixel.R * 4 > pixel.G + pixel.B)
                {
                    r += colors[i].R;
                    total += third;
                }
                if (pixel.G * 4 > pixel.R + pixel.B)
                {
                    g += colors[i].G;
                    total += third;
                }
                if (pixel.B * 4 > pixel.R + pixel.G)
                {
                    b += colors[i].B;
                    total += third;
                }
            }
            return new Color(r / total, g / total, b / total);
        }
    }
}

using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace AATool.Graphics
{
    public static class FontSet
    {
        private static Dictionary<string, FontSystem> systems;
        private static Dictionary<string, Dictionary<int, DynamicSpriteFont>> fonts;

        public static void Initialize(GraphicsDevice device)
        {
            systems = new Dictionary<string, FontSystem>();
            fonts = new Dictionary<string, Dictionary<int, DynamicSpriteFont>>();

            if (!Directory.Exists(Paths.DIR_FONTS))
                return;

            //support both .ttf and .otf font types
            LoadFiles(device, Paths.DIR_FONTS, "ttf");
            LoadFiles(device, Paths.DIR_FONTS, "otf");
        }

        private static void LoadFiles(GraphicsDevice device, string directory, string extension)
        {
            //recursively read all font files
            foreach (string file in Directory.EnumerateFiles(directory, "*." + extension, SearchOption.AllDirectories))
            {
                var font = FromFile(file, device);
                systems[Path.GetFileNameWithoutExtension(file)] = font;
                fonts[Path.GetFileNameWithoutExtension(file)] = new Dictionary<int, DynamicSpriteFont>();
            }
        }

        private static FontSystem FromFile(string file, GraphicsDevice device)
        {
            try
            {
                using (var stream = File.OpenRead(file))
                {
                    var fontSystem = FontSystemFactory.Create(device, 1024, 1024, true);
                    fontSystem.UseKernings = false;
                    fontSystem.AddFont(stream);
                    return fontSystem;
                }
            }
            catch { }
            return null;
        }

        public static DynamicSpriteFont Get(string key, int scale)
        {
            if (fonts.TryGetValue(key ?? string.Empty, out var dynamicFont))
            {
                if (dynamicFont.TryGetValue(scale, out var spriteFont))
                    return spriteFont;
                else
                {
                    //font is loaded, but requested scale isn't cached; create it
                    var font = systems[key].GetFont(scale);
                    fonts[key][scale] = font;
                    return font;
                }
            }
            return null;
        }
    }
}

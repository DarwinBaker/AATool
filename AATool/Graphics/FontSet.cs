using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace AATool.Graphics
{
    public static class FontSet
    {
        private static Dictionary<string, FontSystem> Systems;
        private static Dictionary<string, Dictionary<int, DynamicSpriteFont>> Fonts;

        public static void Initialize()
        {
            Systems = new ();
            Fonts = new ();

            if (Directory.Exists(Paths.System.FontsFolder))
            {
                //support both .ttf and .otf font types
                LoadFiles(Paths.System.FontsFolder, "ttf");
                LoadFiles(Paths.System.FontsFolder, "otf");
            }
        }

        private static void LoadFiles(string directory, string extension)
        {
            //recursively read all font files
            foreach (string file in Directory.EnumerateFiles(directory, "*." + extension, SearchOption.AllDirectories))
            {
                FontSystem font = FromFile(file);
                Systems[Path.GetFileNameWithoutExtension(file)] = font;
                Fonts[Path.GetFileNameWithoutExtension(file)] = new Dictionary<int, DynamicSpriteFont>();
            }
        }

        private static FontSystem FromFile(string file)
        {
            var fontSystem = new FontSystem();
            try
            {
                using FileStream stream = File.OpenRead(file);
                fontSystem.AddFont(stream);
            }
            catch { }
            return fontSystem;
        }

        public static DynamicSpriteFont Get(string key, int scale)
        {
            if (Fonts.TryGetValue(key ?? string.Empty, out Dictionary<int, DynamicSpriteFont> dynamicFont))
            {
                if (dynamicFont.TryGetValue(scale, out DynamicSpriteFont spriteFont))
                {
                    //return cached scale
                    return spriteFont;
                }
                else
                {
                    //font is loaded, but requested scale isn't cached. build it
                    DynamicSpriteFont font = Systems[key].GetFont(scale);
                    Fonts[key][scale] = font;
                    return font;
                }
            }
            return null;
        }
    }
}

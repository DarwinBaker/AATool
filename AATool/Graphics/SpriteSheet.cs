using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.Graphics
{
    public static class SpriteSheet
    {
        const int MaximumWidth = 2048;
        const int InitialHeight = 2048;

        public static RenderTarget2D Atlas { get; private set; }
        public static Rectangle Pixel { get; private set; }

        public static int Width => Atlas?.Width ?? 0;
        public static int Height => Atlas?.Height ?? 0;

        private static readonly Dictionary<string, Sprite> AllSprites = new ();
        private static readonly Dictionary<string, AnimatedSprite> AnimatedSprites = new ();
        private static readonly HashSet<string> LoadedTextureSets = new ();

        private static SpriteBatch InternalBatch;
        private static int CurrentRowHeight = 0;
        private static int CursorX = 0;
        private static int CursorY = 0;

        public static bool TryGet(string key, out Sprite sprite, int width = 0)
        {
            sprite = null;
            if (key is not null && !AllSprites.TryGetValue(key, out sprite) && width > 0)
            {
                //sprite not found by plain name, check for a separate "key^width" variant
                AllSprites.TryGetValue(key + Sprite.ResolutionFlag + width, out sprite);
            }
            return sprite is not null;
        }

        public static bool ContainsSprite(string key) => 
            AllSprites.ContainsKey(key ?? string.Empty);

        public static bool IsAnimated(string key) => 
            AllSprites.TryGetValue(key, out Sprite sprite) && sprite is AnimatedSprite;

        public static void Update(Time time)
        {
            decimal animationTime = time.TotalFrames / 2.5m;
            foreach (AnimatedSprite sprite in AnimatedSprites.Values)
                sprite.Animate(animationTime);
        }

        public static string ExtractAnimation(ref string key, out int frames, out int columns)
        {
            frames = 0;
            columns = 0;
            int index = key.IndexOf(Sprite.FramesFlag);
            if (index is -1)
                return key;

            //parse frame count and optional column count
            string[] tokens = key.Substring(index + 1).Split(Sprite.ColumnsDelimiter);
            int.TryParse(tokens.FirstOrDefault(), out frames);
            if (tokens.Length > 1)
                int.TryParse(tokens[1], out columns);
            else
                columns = frames;

            //remove animation tag from key
            return key = key.Substring(0, index);
        }

        public static string ExtractPadding(ref string key, out int padding)
        {
            padding = 0;
            int index = key.IndexOf(Sprite.PaddingFlag);
            if (index is -1)
                return key;

            int.TryParse(key.Substring(index + 1), out padding);

            //remove padding tag from key
            return key = key.Substring(0, index);
        }

        public static void Initialize()
        {
            InternalBatch = new SpriteBatch(Main.Device);
            Atlas = new RenderTarget2D(Main.Device, MaximumWidth, InitialHeight, false, 
                SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            //get all textures in assets folder and stitch them together in order of descending height
            Stack<Texture2D> textures = GetTexturesRecursive(Paths.System.SpritesFolder);
            textures.Push(NewPixelTexture());
            AppendAtlas(textures.OrderBy(texture => -texture.Height).ToArray());

            //register individual white pixel for faster rendering of solid rectangles
            if (TryGet("pixel", out Sprite pixel))
                Pixel = pixel.Source;
        }

        public static void Require(string textureSet, int requiredHeight)
        {
            //check if set has already been loaded
            if (LoadedTextureSets.Contains(textureSet))
                return;

            //make sure atlas has enough room
            ExpandTo(requiredHeight);

            //get all textures in specified folder and add to atlas in order of descending height
            string path = Path.Combine(Paths.System.SpritesFolder, textureSet);
            Stack<Texture2D> newTextures = GetTexturesRecursive(path);
            if (newTextures.Any())
            {
                AppendAtlas(newTextures.OrderBy(texture => -texture.Height).ToArray());
                LoadedTextureSets.Add(textureSet);
            }
        }

        public static void DumpAtlas()
        {
            try
            {
                string path = Path.Combine(Environment.CurrentDirectory, "atlas_dump.png");
                using (FileStream stream = File.Create(path))
                    Atlas.SaveAsPng(stream, Atlas.Width, Atlas.Height);

                DialogResult result = MessageBox.Show(null,
                    $"The current state of AATool's texture atlas has been saved to the file \"{path}\"." +
                    "Would you like to view the file now?", 
                    "Texture Atlas Dumped",
                    MessageBoxButtons.YesNo);
                if (result is DialogResult.Yes)
                    Process.Start(path);
            }
            catch (Exception e)
            {
                if (e is not IOException or UnauthorizedAccessException)
                    throw;
            }
        }

        public static void AppendAtlas(params Texture2D[] textures)
        {
            lock (Atlas)
            {
                //create spritebatch and prepare rendertarget
                Main.Device.SetRenderTarget(Atlas);
                InternalBatch.Begin();

                //render each texture to the atlas
                foreach (Texture2D texture in textures)
                {
                    //strip away and parse metadata from filename
                    string key = texture.Tag.ToString();
                    ExtractAnimation(ref key, out int frames, out int columns);
                    ExtractPadding(ref key, out int padding);

                    //find optimal position in atlas for this texture
                    Rectangle bounds = Fit(texture, padding);

                    //create new sprite to hold metadata
                    if (frames > 1)
                    {
                        AnimatedSprites[key] = new AnimatedSprite(bounds, frames, columns);
                        AllSprites[key] = AnimatedSprites[key];
                    }
                    else
                    {
                        AllSprites[key] = new Sprite(bounds);
                    }
                    //draw texture to its given rectangle
                    InternalBatch.Draw(texture, AllSprites[key].Source, Color.White);
                }
                InternalBatch.End();
                Main.Device.SetRenderTarget(null);
            }
        }

        private static Stack<Texture2D> GetTexturesRecursive(string topDirectory)
        {
            var textures = new Stack<Texture2D>();
            try
            {
                //recursively read all .png files
                foreach (string file in Directory.EnumerateFiles(topDirectory, "*.png", SearchOption.AllDirectories))
                {
                    if (TryReadTexture(file, out Texture2D texture))
                    {
                        //store filename in tag for use later
                        texture.Tag = Path.GetFileNameWithoutExtension(file);
                        textures.Push(texture);
                    }
                }
            }
            catch
            { 
                //couldn't enumerate files, move on
            }
            return textures;
        }

        public static bool TryReadTexture(string file, out Texture2D texture)
        {
            try
            {
                using (FileStream stream = File.OpenRead(file))
                    texture = Texture2D.FromStream(Main.Device, stream);
            }
            catch
            {
                texture = null;
            }
            return texture is not null;
        }

        public static System.Drawing.Image BitmapFromFile(string file, int width, int height)
        {
            try
            {
                using (var bitmap = new System.Drawing.Bitmap(file))
                    return new System.Drawing.Bitmap(bitmap, width, height);
            }
            catch
            {
                return null;
            }
        }

        private static Texture2D NewPixelTexture()
        {
            //create solid color 1x1 texture for drawing solid rectangles/lines
            var pixel = new Texture2D(Main.Device, 1, 1);
            pixel.SetData(new Color[1] { Color.White });
            pixel.Tag = "pixel";
            return pixel;
        }

        private static Rectangle Fit(Texture2D texture, int padding)
        {
            if (texture is null)
                return Rectangle.Empty;

            //find next rectangle that will fit the given texture on the atlas
            if (CursorX + texture.Width > Width && CursorX > 0)
            {
                //row is full, move cursor to beginning of next row
                CursorX = padding;
                CursorY += CurrentRowHeight + padding;
                CurrentRowHeight = 0;
            }

            //make sure atlas has room
            if (CursorY + texture.Height <= Height)
            {
                var bounds = new Rectangle(
                    CursorX + padding,
                    CursorY + padding,
                    texture.Width,
                    texture.Height);

                //move cursor cursor
                CursorX += texture.Width + (padding * 2);
                CurrentRowHeight = Math.Max(CurrentRowHeight, texture.Height + (padding * 2));
                return bounds;
            }

            //texture didn't fit. should never happen in practice
            return Rectangle.Empty;
        }

        private static void ExpandTo(int requiredHeight)
        {
            if (Height >= requiredHeight)
                return;

            lock (Atlas)
            {
                //store current render target temporarily
                RenderTarget2D oldAtlas = Atlas;

                //create new render target of required size
                Atlas = new RenderTarget2D(Main.Device, MaximumWidth, requiredHeight, false,
                    SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                //add old render target contents to new one
                Main.Device.SetRenderTarget(Atlas);
                InternalBatch.Begin();
                InternalBatch.Draw(oldAtlas, Vector2.Zero, Color.White);
                InternalBatch.End();
                Main.Device.SetRenderTarget(null);
            }
        }
    }
}

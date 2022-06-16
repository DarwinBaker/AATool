using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AATool.Data.Categories;
using AATool.Net;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.Graphics
{
    public static class SpriteSheet
    {
        const string AvatarPrefix = "avatar-";
        const int MaximumSize = 4096;
        const int InitialSize = 2048;
        const int ExpansionIncrement = 256;

        public static RenderTarget2D Atlas { get; private set; }
        public static Sprite Pixel { get; private set; }

        private static readonly Dictionary<string, Sprite> AllSprites = new ();
        private static readonly Dictionary<string, AnimatedSprite> AnimatedSprites = new ();
        private static readonly List<string> LoadedTextureSets = new ();

        private static SpriteBatch InternalBatch;
        private static int ActiveRowHeight = 0;
        private static int OffsetX = 0;
        private static int OffsetY = 0;
        private static int StartX = 0;

        private static int Width => StartX is 0 ? InitialSize : MaximumSize;

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
            decimal animationTime = time.TotalFrames / 3;
            lock (AnimatedSprites)
            {
                foreach (AnimatedSprite sprite in AnimatedSprites.Values)
                    sprite.Animate(animationTime);
            }
        }

        public static void Initialize()
        {
            InternalBatch = new SpriteBatch(Main.Device);
            Atlas = new RenderTarget2D(Main.Device, InitialSize, InitialSize, false,
                SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            lock (LoadedTextureSets)
            {
                //load textures required at launch
                string globalSprites = Path.Combine(Paths.System.SpritesFolder, "global");
                Stack<Texture2D> textures = GetTexturesRecursive(globalSprites);
                Pack(textures.ToArray());
                Dispose(textures);

                //register individual white pixel for fast rendering of solid rectangles
                if (TryGet("pixel", out Sprite pixel))
                    Pixel = pixel;

                //get cached player colors
                string avatarSprites = Path.Combine(globalSprites, "avatar_cache");
                textures = GetTexturesRecursive(avatarSprites);
                foreach (Texture2D texture in textures)
                {
                    string key = texture.Tag as string;
                    if (string.IsNullOrEmpty(key) || key.Length <= AvatarPrefix.Length)
                        continue;

                    string identifier = key.Substring(AvatarPrefix.Length);
                    if (Uuid.TryParse(identifier, out Uuid id))
                        Player.Cache(id, ColorHelper.GetAccent(texture));
                    else
                        Player.Cache(identifier, ColorHelper.GetAccent(texture));
                }
            }
        }

        public static void Require(string textureSet)
        {
            //check if set has already been loaded
            if (LoadedTextureSets.Contains(textureSet))
                return;

            //get all textures in specified folder and add to atlas in order of descending height
            string path = Path.Combine(Paths.System.SpritesFolder, textureSet);
            Stack<Texture2D> textures = GetTexturesRecursive(path);
            if (textures.Any())
            {
                Pack(textures.ToArray());
                LoadedTextureSets.Add(textureSet);
                Dispose(textures);
            }
        }

        public static void Include(string textureSet)
        {
            new Thread(() => {

                Require(textureSet);

                if (textureSet is "blocks")
                    AllBlocks.SpritesLoaded = true;

            }).Start();
        }

        private static bool TryReadTexture(string file, out Texture2D texture)
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

        public static void Pack(params Texture2D[] textures)
        {
            lock (AllSprites) 
            lock (AnimatedSprites)
            {
                //sort textures from largest to smallest, first by height then by width
                IOrderedEnumerable<Texture2D> sorted = textures
                .OrderBy(texture => -texture.Height)
                .ThenBy(texture => -texture.Width);

                if (sorted.FirstOrDefault()?.Height > ActiveRowHeight)
                {
                    //start new row to save space
                    OffsetX = StartX;
                    OffsetY += ActiveRowHeight;
                    ActiveRowHeight = 0;
                }

                foreach (Texture2D texture in sorted)
                {
                    //strip away and parse metadata from filename
                    string key = Sprite.ParseId(texture.Tag.ToString(),
                    out int padding,
                    out int frames,
                    out int columns,
                    out decimal speed);

                    //find (semi)optimal position in atlas unless already loaded
                    if (!AllSprites.ContainsKey(key) && Fit(texture, padding, out Rectangle bounds))
                    {
                        //create new sprite to store metadata
                        Sprite sprite = frames > 1
                        ? AnimatedSprites[key] = new AnimatedSprite(bounds, frames, columns, speed)
                        : new Sprite(bounds);
                        AllSprites[key] = sprite;

                        //store new sprite in texture tag
                        texture.Tag = sprite;
                    }
                }

                //add new textures to the atlas
                Render(textures);
            }
        }

        public static void Replace(string key, Texture2D replacement)
        {
            if (TryGet(key, out Sprite existing))
            {
                lock (Atlas)
                {
                    //create spritebatch and prepare rendertarget
                    Main.Device.SetRenderTarget(Atlas);
                    InternalBatch.Begin();

                    //draw over existing texture
                    InternalBatch.Draw(replacement, existing.Source, Color.White);

                    InternalBatch.End();
                    Main.Device.SetRenderTarget(null);
                }
            }
        }

        private static void Render(params Texture2D[] textures)
        {
            //make sure atlas texture is big enough to render to
            ExpandRenderTarget(OffsetY + ActiveRowHeight);

            lock (Atlas)
            {
                //create spritebatch and prepare rendertarget
                Main.Device.SetRenderTarget(Atlas);
                InternalBatch.Begin();

                //render each texture to the atlas
                foreach (Texture2D texture in textures)
                {
                    //draw texture to its cooresponding sprite bounds
                    if (texture?.Tag is Sprite sprite)
                        InternalBatch.Draw(texture, sprite.Source, Color.White);
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

        private static bool Fit(Texture2D texture, int padding, out Rectangle bounds)
        {
            //validate texture
            bounds = default;
            if (texture is null)
                return false;

            int consumedWidth = texture.Width + (padding * 2);
            int consumedHeight = texture.Height + (padding * 2);
            ActiveRowHeight = Math.Max(ActiveRowHeight, consumedHeight);

            int right = OffsetX + consumedWidth;
            if (right > Width)
            {
                //start new row
                OffsetX = StartX;
                OffsetY += ActiveRowHeight;
                ActiveRowHeight = consumedHeight;
            }

            int bottom = OffsetY + ActiveRowHeight;
            if (bottom > MaximumSize)
            {
                //expand horizontally and start at top
                StartX = InitialSize;
                OffsetX = StartX;
                OffsetY = 0;
            }

            //calculate bounds for this texture
            bounds = new Rectangle(
                OffsetX + padding,
                OffsetY + padding,
                texture.Width,
                texture.Height);

            //move to the right
            OffsetX += consumedWidth;
            return bounds != default;
        }

        private static void ExpandRenderTarget(int bottom)
        {
            //nothing to do if atlas isn't initialized yet
            if (Atlas is null)
                return;

            int newHeight = (int)(Math.Ceiling((decimal)bottom / ExpansionIncrement) * ExpansionIncrement);
            if (StartX > 0)
                newHeight = MaximumSize;

            //nothing to do if height is already enough or is max size
            if (Width == Atlas?.Width && newHeight == Atlas?.Height)
                return;

            lock (Atlas)
            {
                //store existing render target temporarily
                RenderTarget2D oldAtlas = Atlas;

                //create new render target of required size
                Atlas = new RenderTarget2D(Main.Device, Width, newHeight, false,
                    SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                //re-add old render target contents to new one
                Main.Device.SetRenderTarget(Atlas);
                InternalBatch.Begin();
                InternalBatch.Draw(oldAtlas, Vector2.Zero, Color.White);
                InternalBatch.End();
                Main.Device.SetRenderTarget(null);
            }
        }

        private static void Dispose(IEnumerable<Texture2D> textures)
        {
            foreach (Texture2D texture in textures)
                texture?.Dispose();
        }

        public static void DumpAtlas()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "atlas_dump.png");
            try
            {
                using (FileStream stream = File.Create(path))
                    Atlas.SaveAsPng(stream, Atlas.Width, Atlas.Height);

                DialogResult result = MessageBox.Show(null,
                    $"The current state of AATool's texture atlas has been saved to the file \"{path}\"." +
                    "Would you like to view the file now?", "Texture Atlas Dumped", MessageBoxButtons.YesNo);
                if (result is DialogResult.Yes)
                    _ = Process.Start(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(null, $"Error saving AATool's texture atlas to file \"{path}\".\n\n" + e.Message,
                    "Texture Atlas Dumped", MessageBoxButtons.OK);
            }
        }
    }
}

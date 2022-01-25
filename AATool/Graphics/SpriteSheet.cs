using AATool.Net;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace AATool.Graphics
{
    public static class SpriteSheet
    {
        public const int WIDTH  = 2048;
        public const int HEIGHT = 2048;
        public const int MAX_COLUMNS = 32;

        public const string ANIMATION_PREFIX  = "$";
        public const string ResolutionPrefix = "^";
        public const string PADDING_PREFIX    = "~";

        public static RenderTarget2D Atlas;
        public static Dictionary<string, Sprite> Sprites = new ();

        private static GraphicsDevice Device;
        private static int CursorX           = 0;
        private static int CursorY           = 0;
        private static int RowHeight         = 0;
        private static decimal AnimationTime = 0;

        public static Sprite Pixel => Sprites["pixel"];

        public static bool ContainsSprite(string id) => Sprites.ContainsKey(id ?? string.Empty);
        public static bool IsAnimated(string id) => Sprites.TryGetValue(id, out Sprite sprite) && sprite.Frames > 0;
        public static string ProcessFrameCount(string id, out int frames)
        {
            frames = 0;
            int index = id.IndexOf(ANIMATION_PREFIX);
            if (index is -1)
                return id;

            int.TryParse(id.Substring(index + 1), out frames);
            return id.Substring(0, index);
        }

        public static string ProcessPadding(string id, out int padding)
        {
            padding = 0;
            int index = id.IndexOf(PADDING_PREFIX);
            if (index is -1)
                return id;

            int.TryParse(id.Substring(index + 1), out padding);
            return id.Substring(0, index);
        }

        public static bool TryGetRectangle(string id, out Rectangle rectangle)
        {
            //find the atlas rectangle of a sprite by id
            if (!Sprites.TryGetValue(id, out Sprite sprite))
            {
                rectangle = Rectangle.Empty;
                return false;
            }

            if (sprite.Frames < 2)
            {
                //static texture
                rectangle = sprite.Source;
            }
            else
            {
                //animated texture
                int columns = Math.Min(sprite.Frames, MAX_COLUMNS);
                int width   = sprite.Width / columns;
                int height  = sprite.Height / (int)Math.Ceiling(sprite.Frames / (double)columns);
                int wrapped = GetFrameIndex(sprite.Frames);
                int x       = wrapped % columns * width;
                int y       = wrapped / columns * height;
                rectangle   = new(sprite.Source.Location + new Point(x, y), new Point(width, width));
            }
            return true;
        }

        public static int GetFrameIndex(int frameCount)
        {
            if (frameCount < 1)
                return 0;
            //get the current animation frame index for a texture based on how many it has
            decimal loops = Math.Floor(AnimationTime / frameCount);
            return (int)(AnimationTime - (loops * frameCount));
        }

        public static Point GetFrameOffset(Sprite sprite)
        {
            //get the current animation frame for a texture based on how many it has
            int columns = Math.Min(sprite.Frames, MAX_COLUMNS);
            int width   = sprite.Width / columns;
            int height  = sprite.Height / (sprite.Frames / columns);
            int wrapped = GetFrameIndex(sprite.Frames);
            int x = wrapped % columns * width;
            int y = wrapped / columns * height;
            return new Point(x, y);
        }

        public static void Update(Time time) => AnimationTime = time.TotalFrames / 2.5m;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            Device = graphicsDevice;
            Atlas  = new RenderTarget2D(Device, WIDTH, HEIGHT, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            if (!Directory.Exists(Paths.System.SpritesFolder))
                return;

            //get all textures in assets folder and stitch them together in order of descending height
            Stack<Texture2D> textures = LoadFiles(Paths.System.SpritesFolder);
            textures.Push(CreatePixelTexture(Device));
            AppendAtlas(textures.OrderBy(texture => -texture.Height).ToArray());
        }

        public static void DumpAtlas()
        {
            try
            {
                using (FileStream stream = File.Create("atlas_dump.png"))
                    Atlas.SaveAsPng(stream, Atlas.Width, Atlas.Height);
            }
            catch (Exception e)
            {
                if (e is not IOException or UnauthorizedAccessException)
                    throw;
            }
        }

        private static Stack<Texture2D> LoadFiles(string directory)
        {
            //recursively read all .png files into Texture2D objects
            var textures = new Stack<Texture2D>();
            foreach (string file in Directory.EnumerateFiles(directory, "*.png", SearchOption.AllDirectories))
            {
                if (TryGetTextureFromFile(file, out Texture2D texture))
                {
                    //store filename in tag for use later
                    texture.Tag = Path.GetFileNameWithoutExtension(file);
                    textures.Push(texture);
                }
            }
            return textures;
        }

        public static bool TryGetTextureFromFile(string file, out Texture2D texture)
        {
            texture = null;
            if (File.Exists(file))
            {
                try
                {
                    using (FileStream stream = File.OpenRead(file))
                        texture = Texture2D.FromStream(Device, stream);
                    return true;
                }
                catch { }
            }
            return false;
        }

        public static System.Drawing.Image BitmapFromFile(string file, int width, int height)
        {
            try
            {
                using var bitmap = new System.Drawing.Bitmap(file);
                return new System.Drawing.Bitmap(bitmap, width, height);
            }
            catch { }
            return null;
        }

        public static void AppendAtlas(params Texture2D[] textures)
        {
            lock (Atlas)
            {
                //create spritebatch and prepare rendertarget
                var batch = new SpriteBatch(Device);
                Device.SetRenderTarget(Atlas);
                batch.Begin();

                //render each texture to the atlas
                foreach (Texture2D texture in textures)
                {
                    string id = texture.Tag.ToString();
                    id = ProcessFrameCount(id, out int frames);
                    id = ProcessPadding(id, out int padding);

                    if (!ContainsSprite(id))
                    {
                        //draw texture in its given rectangle
                        Rectangle rectangle = NextRectangle(texture, padding);
                        Sprites.Add(id, new Sprite(rectangle, frames));
                        batch.Draw(texture, rectangle, Color.White);
                    }
                }
                batch.End();
                Device.SetRenderTarget(null);
            }
        }

        private static Texture2D CreatePixelTexture(GraphicsDevice device)
        {
            //procedural solid color texture for drawing rectangles and lines
            var pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new Color[1] { Color.White });
            pixel.Tag = "pixel";
            return pixel;
        }

        private static Rectangle NextRectangle(Texture2D texture, int padding)
        {
            //find next rectangle that will fit the given texture on the atlas
            if (texture is null)
                return Rectangle.Empty;

            if (CursorX + texture.Width > WIDTH)
            {
                //row is full, set cursor to start of next row
                CursorX = padding;
                CursorY += RowHeight + padding;
                RowHeight = 0;
            }

            //give up if no room in atlas
            if (CursorY + texture.Height > HEIGHT)
                return Rectangle.Empty;

            var rectangle = new Rectangle(
                CursorX + padding,
                CursorY + padding,
                texture.Width,
                texture.Height);

            //offset cursor
            CursorX += texture.Width + (padding * 2);
            RowHeight = Math.Max(RowHeight, texture.Height + (padding * 2));
            return rectangle;
        }
    }
}

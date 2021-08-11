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
        public const int RESOLUTION  = 2048;
        public const int MAX_COLUMNS = 32;

        public const string ANIMATION_PREFIX  = "$";
        public const string RESOLUTION_PREFIX = "^";

        public static RenderTarget2D Atlas;
        public static Dictionary<string, Sprite> Sprites = new ();

        private static GraphicsDevice Device;
        private static int CursorX           = 0;
        private static int CursorY           = 0;
        private static int RowHeight         = 0;
        private static decimal AnimationTime = 0;

        public static readonly object AtlasLock = new object();

        public static Sprite Pixel => Sprites["pixel"];

        public static bool ContainsSprite(string id) => Sprites.ContainsKey(id ?? string.Empty);
        public static bool IsAnimated(string id)     => id.Contains(ANIMATION_PREFIX);

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
                rectangle   = new (sprite.Source.Location + new Point(x, y), new Point(width, width));
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
            Device  = graphicsDevice;
            Atlas   = new RenderTarget2D(Device, RESOLUTION, RESOLUTION, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            if (!Directory.Exists(Paths.DIR_SPRITES))
                return;

            //get all textures in assets folder and stitch them together in order of descending height
            Stack<Texture2D> textures = LoadFiles(Paths.DIR_SPRITES);
            textures.Push(CreatePixelTexture(Device));
            AppendAtlas(textures.OrderBy(texture => -texture.Height).ToArray());
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
                    int frameCount = 1;
                    if (IsAnimated(id))
                    {
                        //texture is animated; extract and strip suffix from id
                        int prefixIndex = id.IndexOf('$');
                        if (int.TryParse(id.Substring(prefixIndex + 1), out frameCount))
                            id = id.Substring(0, prefixIndex);
                    }

                    if (!ContainsSprite(id))
                    {
                        //draw texture in its given rectangle
                        Rectangle rectangle = NextRectangle(texture);
                        Sprites.Add(id, new Sprite(rectangle, frameCount));
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
        /*
        private static Rectangle Fit(Texture2D sprite)
        {
            if (TryFindNode(Root, sprite.Width, sprite.Height, out Rectangle node))
                sprite.fit = SplitNode(node, sprite.Width, sprite.Height);
        }

        private static bool TryFindNode(Rectangle root, Texture2D sprite, out Rectangle rectangle)
        {
            if (root.used)
            {
                if (TryFindNode(root.right, w, h) || TryFindNode(root.down, w, h))
                {

                }
                return ;
            }
            else if ((sprite.Width <= root.Width) && (sprite.Height <= root.Height))
            {
                rectangle = root;
                return true;
            } 
            else
            {
                rectangle = Rectangle.Empty;
                return false;
            }
        }

        private static Rectangle SplitNode(node, int width, int height)
        {
            node.used = true;
            node.down  = { x: node.x,     y: node.y + h, w: node.w,     h: node.h - h };
            node.right = { x: node.x + w, y: node.y,     w: node.w - w, h: h          };
            return node;
        }
        */
        private static Rectangle NextRectangle(Texture2D texture)
        {
            //find next rectangle that will fit the given texture on the atlas
            if (texture is null)
                return Rectangle.Empty;

            if (CursorX + texture.Width > RESOLUTION)
            {
                //row is full, set cursor to start of next row
                CursorX = 0;
                CursorY += RowHeight;
                RowHeight = 0;
            }

            //give up if no room in atlas
            if (CursorY + texture.Height > RESOLUTION)
                return Rectangle.Empty;

            var rectangle = new Rectangle(CursorX, CursorY, texture.Width, texture.Height);

            //offset cursor
            CursorX += texture.Width;
            RowHeight = Math.Max(RowHeight, texture.Height);
            return rectangle;
        }
    }
}

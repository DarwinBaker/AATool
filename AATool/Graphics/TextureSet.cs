using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AATool.Graphics
{
    public static class TextureSet
    {
        public const int RESOLUTION          = 2048;
        public const int MAX_ANIM_COLUMNS    = 32;
        public const int ANIM_FRAME_SIZE     = 16;
        public const string ANIMATION_PREFIX = "$";

        public static RenderTarget2D Atlas;
        public static Dictionary<string, Rectangle> SubTextures;
        public static Dictionary<string, int> FrameCounts;

        private static int currentX  = 0;
        private static int currentY  = 0;
        private static int rowHeight = 0;
        private static decimal animationTime = 0;

        public static Rectangle RectangleOf(string id)
        {
            //find the atlas rectangle of a texture by id
            SubTextures.TryGetValue(id ?? "", out var source);
            if (FrameCounts.TryGetValue(id, out int count))
                return new Rectangle(source.Location + GetFrameOffset(count), new Point(16));
            return source;
        }

        private static Point GetFrameOffset(int frameCount)
        {
            //get the current animation frame for a texture based on how many it has
            decimal loops = Math.Floor(animationTime / frameCount);
            int wrapped = (int)(animationTime - (loops * frameCount));
            int x = (wrapped % MAX_ANIM_COLUMNS) * ANIM_FRAME_SIZE;
            int y = (wrapped / MAX_ANIM_COLUMNS) * ANIM_FRAME_SIZE;
            return new Point(x, y);
        }

        public static void Update(Time time)
        {
            animationTime = (time.TotalFrames / 3m);
        }

        public static void Initialize(GraphicsDevice device)
        {
            Atlas       = new RenderTarget2D(device, RESOLUTION, RESOLUTION, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            SubTextures = new Dictionary<string, Rectangle>();
            FrameCounts = new Dictionary<string, int>();

            if (!Directory.Exists(Paths.DIR_TEXTURES))
                return;

            //get all textures in assets folder and stitch them together in order of descending size
            var textures = LoadFiles(device, Paths.DIR_TEXTURES);
            StitchAtlas(device, textures.OrderBy(texture => -texture.Width * texture.Height).ToList());
        }

        private static List<Texture2D> LoadFiles(GraphicsDevice device, string directory)
        {
            //recursively read all .png files into Texture2D objects
            var textures = new List<Texture2D>();
            foreach (string file in Directory.EnumerateFiles(directory, "*.png", SearchOption.AllDirectories))
            {
                var texture = TextureFromFile(file, device);
                if (texture != null)
                {
                    //store filename in tag for use later
                    texture.Tag = Path.GetFileNameWithoutExtension(file);
                    textures.Add(texture);
                }
            }
            return textures;
        }

        public static Texture2D TextureFromFile(string file, GraphicsDevice device)
        {
            try
            {
                using (var stream = File.OpenRead(file))
                    return Texture2D.FromStream(device, stream);
            }
            catch { }
            return null;
        }

        public static System.Drawing.Image BitmapFromFile(string file, int width, int height)
        {
            try
            {
                using (var bitmap = new System.Drawing.Bitmap(file))
                    return new System.Drawing.Bitmap(bitmap, width, height);
            }
            catch { }
            return null;
        }

        private static void StitchAtlas(GraphicsDevice device, IList<Texture2D> textures)
        {
            //create spritebatch and prepare rendertarget
            var batch = new SpriteBatch(device);
            device.SetRenderTarget(Atlas);
            device.Clear(Color.Transparent);
            batch.Begin();

            //render each texture to the atlas
            foreach (var texture in textures)
            {
                string id = texture.Tag.ToString();
                if (id.Contains(ANIMATION_PREFIX))
                {
                    //texture is animated; add frame count to dictionary
                    int prefixIndex = id.IndexOf('$');
                    if (int.TryParse(id.Substring(prefixIndex + 1), out int count))
                    {
                        id = id.Substring(0, prefixIndex);
                        FrameCounts[id] = count;
                    }
                }

                if (!SubTextures.ContainsKey(id))
                {
                    //draw texture in its given rectangle
                    var rectangle = NextRectangle(texture);
                    SubTextures.Add(id, rectangle);
                    batch.Draw(texture, rectangle, Color.White);
                }
            }
            
            //create and add solid pixel brush
            var pixel = CreateSolidColorTexture(device);
            var pixelRectangle = NextRectangle(pixel);
            SubTextures.Add("pixel", pixelRectangle);
            batch.Draw(pixel, pixelRectangle, Color.White);
            batch.End();
            device.SetRenderTarget(null);
        }

        private static Texture2D CreateSolidColorTexture(GraphicsDevice device)
        {
            //procedural solid color texture for drawing rectangles and lines
            var pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new Color[1] { Color.White });
            return pixel;
        }

        private static Rectangle NextRectangle(Texture2D texture)
        {
            //find next rectangle that will fit the given texture on the atlas

            if (texture == null)
                return Rectangle.Empty;

            int width = texture.Width;
            int height = texture.Height;
            if (currentX + width > RESOLUTION)
            {
                //row is full, go to start of next row
                currentX = 0;
                currentY += rowHeight;
                rowHeight = 0;
            }

            //give up if no room in atlas
            if (currentY + height > RESOLUTION)
                return Rectangle.Empty;

            var rectangle = new Rectangle(currentX, currentY, width, height);
            currentX += width;
            rowHeight = Math.Max(rowHeight, height);
            return rectangle;
        }
    }
}

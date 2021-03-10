using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AATool.Graphics
{
    public static class SpriteSheet
    {
        public const int RESOLUTION           = 2048;
        public const int ANIM_COLUMNS         = 32;
        public const string ANIMATION_PREFIX  = "$";
        public const string RESOLUTION_PREFIX = "^";

        public static RenderTarget2D Atlas;
        public static Dictionary<string, Sprite> Sprites;

        private static int cursorX  = 0;
        private static int cursorY  = 0;
        private static int rowHeight = 0;
        private static decimal animationTime = 0;

        public static Rectangle RectangleOf(string id)
        {
            //find the atlas rectangle of a texture by id
            Sprites.TryGetValue(id, out var texure);
            if (texure.FrameCount < 2)
                return texure.SourceRectangle;
            else
                return new Rectangle(texure.SourceRectangle.Location + GetFrameOffset(texure.FrameCount, texure.SourceRectangle.Width / ANIM_COLUMNS), new Point(texure.SourceRectangle.Width / ANIM_COLUMNS));
        }

        public static int GetFrameIndex(int frameCount, int frameSize)
        {
            if (frameCount < 1)
                return 0;
            //get the current animation frame index for a texture based on how many it has
            decimal loops = Math.Floor(animationTime / frameCount);
            return (int)(animationTime - (loops * frameCount));
        }

        public static Point GetFrameOffset(int frameCount, int frameSize)
        {
            //get the current animation frame for a texture based on how many it has
            int wrapped = GetFrameIndex(frameCount, frameSize);
            int x = (wrapped % ANIM_COLUMNS) * frameSize;
            int y = (wrapped / ANIM_COLUMNS) * frameSize;
            return new Point(x, y);
        }

        public static void Update(Time time)
        {
            animationTime = (time.TotalFrames / 2.5m);
        }

        public static void Initialize(GraphicsDevice device)
        {
            Sprites = new Dictionary<string, Sprite>();
            Atlas = new RenderTarget2D(device, RESOLUTION, RESOLUTION, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

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
                int frameCount = 1;
                if (id.Contains(ANIMATION_PREFIX))
                {
                    //texture is animated; add frame count to dictionary and strip suffix from id
                    int prefixIndex = id.IndexOf('$');
                    if (int.TryParse(id.Substring(prefixIndex + 1), out frameCount))
                        id = id.Substring(0, prefixIndex);
                }

                if (!Sprites.ContainsKey(id))
                {
                    //draw texture in its given rectangle
                    var rectangle = NextRectangle(texture);
                    Sprites.Add(id, new Sprite(rectangle, frameCount));
                    batch.Draw(texture, rectangle, Color.White);
                }
            }
            
            //create and add solid pixel brush
            var pixel = CreateSolidColorTexture(device);
            var pixelRectangle = NextRectangle(pixel);
            Sprites.Add("pixel", new Sprite(pixelRectangle, 1));
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
            if (cursorX + width > RESOLUTION)
            {
                //row is full, go to start of next row
                cursorX = 0;
                cursorY += rowHeight;
                rowHeight = 0;
            }

            //give up if no room in atlas
            if (cursorY + height > RESOLUTION)
                return Rectangle.Empty;

            var rectangle = new Rectangle(cursorX, cursorY, width, height);
            cursorX += width;
            rowHeight = Math.Max(rowHeight, height);
            return rectangle;
        }
    }
}

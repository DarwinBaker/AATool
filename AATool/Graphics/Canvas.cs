using System;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.UI.Screens;
using AATool.Utilities;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.Graphics
{
    public class Canvas
    {
        const string AmbientGlowTexture = "glow_ambient";

        public static void Initialize()
        {
            Batches = new SpriteBatch[Enum.GetNames(typeof(Layer)).Length];
            foreach (int layer in Enum.GetValues(typeof(Layer)))
                Batches[layer] = new SpriteBatch(Main.Device);
            CacheBatch = new SpriteBatch(Main.Device);

            //configure graphics parameters
            Main.GraphicsManager.GraphicsProfile = GraphicsProfile.Reach;
            Main.GraphicsManager.SynchronizeWithVerticalRetrace = true;
            Main.GraphicsManager.HardwareModeSwitch = false;
            Main.GraphicsManager.ApplyChanges();
        }

        private static SpriteBatch[] Batches;
        private static SpriteBatch CacheBatch;

        public static Color RainbowFast { get; private set; }
        public static Color RainbowLight { get; private set; }
        public static Color RainbowStrong { get; private set; }

        public static int GlobalDrawCalls { get; private set; }

        public int ScreenDrawCalls { get; private set; }

        public Canvas()
        {
            
        }

        public static void Update(Time time)
        {
            //fade rainbow to next color
            RainbowFast = ColorHelper.FromHSV(time.TotalFrames % 360, 0.5, 1.0);
            RainbowLight = ColorHelper.FromHSV(time.TotalFrames / 4 % 360, 0.33, 1.0);
            RainbowStrong = ColorHelper.FromHSV(time.TotalFrames / 8 % 360, 1.0, 1.0);
        }

        private static SpriteBatch BatchOf(Layer layer) => Batches[(int)layer];

        public void BeginDraw(UIScreen screen, BlendState blend = null)
        {
            //main layer
            if (screen is not UIMainScreen || UIMainScreen.Invalidated)
            {
                UIMainScreen.RefreshingCache = screen is UIMainScreen;
                BatchOf(Layer.Main).Begin(SpriteSortMode.Deferred,
                    blend ?? BlendState.NonPremultiplied,
                    SamplerState.PointClamp);
            }

            if (screen is UIMainScreen)
            {
                //used to draw render cached to the backbuffer 
                CacheBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.Opaque,
                    SamplerState.PointClamp);
            }

            //lighting layer
            BatchOf(Layer.Glow).Begin(SpriteSortMode.Deferred, 
                BlendState.Additive, 
                SamplerState.AnisotropicClamp);

            //foreground and animated sprites layer
            BatchOf(Layer.Fore).Begin(SpriteSortMode.Deferred, 
                blend ?? BlendState.NonPremultiplied, 
                SamplerState.PointClamp);

            GlobalDrawCalls = 0;
            this.ScreenDrawCalls = 0;
        }

        public void EndDraw(UIScreen screen)
        {
            if (screen is UIMainScreen)
            {
                //main screen makes use of a render cache for improved performance
                if (UIMainScreen.RefreshingCache)
                {
                    //clear cache texture to proper back color
                    Main.Device.SetRenderTarget(UIMainScreen.RenderCache);
                    Main.Device.Clear(Tracker.Category is AllBlocks 
                        ? Config.Main.BorderColor 
                        : Config.Main.BackColor);

                    //render main layer to cache texture
                    BatchOf(Layer.Main).End();
                    Main.Device.SetRenderTarget(null);
                }

                //render cache texture to backbuffer
                if (!Config.Main.HideRenderCache)
                    CacheBatch.Draw(UIMainScreen.RenderCache, Main.Device.Viewport.Bounds, Color.White);
                CacheBatch.End();
            }
            else
            {
                //render main layer directly to viewport
                BatchOf(Layer.Main).End();
            }

            //render ambient glow effect to viewport if enabled
            if (Config.Main.ShowAmbientGlow && screen is not UIOverlayScreen)
                this.Draw(AmbientGlowTexture, Main.Device.Viewport.Bounds, RainbowStrong * 0.5f, Layer.Glow);

            //render lighting to viewport
            BatchOf(Layer.Glow).End();

            //render foreground/animated sprites to viewport
            BatchOf(Layer.Fore).End();
        }

        public void Draw(Texture2D texture, Rectangle destination, 
            Color? tint = null, 
            Layer layer = Layer.Main)
        {
            if (texture is not null)
            {
                //draw a texture that isn't part of the main atlas
                BatchOf(layer).Draw(texture, 
                    destination, 
                    tint ?? Color.White);

                GlobalDrawCalls++;
                this.ScreenDrawCalls++;
            }
        }

        public void Draw(string texture, Rectangle destination, 
            Color? tint = null, 
            Layer layer = Layer.Main)
        {
            if (SpriteSheet.TryGet(texture, out Sprite sprite, destination.Width))
            {
                //draw a portion of the texture atlas
                BatchOf(layer).Draw(SpriteSheet.Atlas,
                    destination,
                    sprite.Source,
                    tint ?? Color.White);

                GlobalDrawCalls++;
                this.ScreenDrawCalls++;
            }
        }

        public void Draw(string texture, Rectangle destination, Rectangle subSource, 
            Color? tint = null, 
            Layer layer = Layer.Main)
        {
            if (SpriteSheet.TryGet(texture, out Sprite sprite, destination.Width))
            {
                var translated = new Rectangle(sprite.Offset + subSource.Location, subSource.Size);

                //draw a sub-portion of a source rectangle of the texture atlas
                BatchOf(layer).Draw(SpriteSheet.Atlas,
                    destination,
                    translated,
                    tint ?? Color.White);

                GlobalDrawCalls++;
                this.ScreenDrawCalls++;
            }
        }

        public void Draw(string texture, Vector2 center, float rotation, 
            Vector2? scale = null, 
            Color? tint = null, 
            Layer layer = Layer.Main)
        {
            if (SpriteSheet.TryGet(texture, out Sprite sprite))
            {
                BatchOf(layer).Draw(SpriteSheet.Atlas, 
                    center, 
                    sprite.Source, 
                    tint ?? Color.White, 
                    rotation, 
                    sprite.Origin, 
                    scale ?? Vector2.One, 
                    SpriteEffects.None, 0);
                         
                GlobalDrawCalls++;
                this.ScreenDrawCalls++;
            }
        }

        public void DrawRectangle(Rectangle rectangle, Color color, 
            Color? borderColor = null, 
            int border = 0, 
            Layer layer = Layer.Main)
        {
            SpriteBatch batch = BatchOf(layer);
            borderColor ??= color;
            if (border > 0 && borderColor != color)
            {
                //draw rectangle with border
                batch.Draw(SpriteSheet.Atlas, rectangle, SpriteSheet.Pixel, borderColor.Value);
                var inner = new Rectangle(rectangle.X + border, rectangle.Y + border, rectangle.Width - border * 2, rectangle.Height - border * 2);
                batch.Draw(SpriteSheet.Atlas, inner, SpriteSheet.Pixel, color);

                //draw rounded corners
                Color blended = ColorHelper.Blend(borderColor.Value, color, 0.5f);
                batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Left + border, rectangle.Top + border, 1, 1), SpriteSheet.Pixel, blended);
                batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Right - (border + 1), rectangle.Top + border, 1, 1), SpriteSheet.Pixel, blended);
                batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Right - (border + 1), rectangle.Bottom - (border + 1), 1, 1), SpriteSheet.Pixel, blended);
                batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Left + border, rectangle.Bottom - (border + 1), 1, 1), SpriteSheet.Pixel, blended);
                GlobalDrawCalls += 6;
                this.ScreenDrawCalls += 6;
            }
            else
            {
                batch.Draw(SpriteSheet.Atlas, rectangle, SpriteSheet.Pixel, color);
                GlobalDrawCalls++;
                this.ScreenDrawCalls++;
            }
        }

        public void DrawString(DynamicSpriteFont font, string text, Vector2 location, Color color, Layer layer = Layer.Main)
        {
            BatchOf(layer).DrawString(font, text, location, color);
            GlobalDrawCalls++;
            this.ScreenDrawCalls++;
        }
    }
}

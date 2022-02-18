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
        public int DrawCalls { get; private set; }
        public Color RainbowFast { get; private set; }
        public Color RainbowLight { get; private set; }
        public Color RainbowStrong { get; private set; }

        private readonly GraphicsDeviceManager deviceManager;
        private readonly GraphicsDevice device;
        private readonly SpriteBatch[] batches;
        private readonly SpriteBatch final;
        
        private SpriteBatch BatchOf(Layer layer) => this.batches[(int)layer];

        public Canvas(GraphicsDeviceManager deviceManager)
        {
            this.deviceManager = deviceManager;
            this.device = this.deviceManager.GraphicsDevice;
            this.batches = new SpriteBatch[Enum.GetNames(typeof(Layer)).Length];
            foreach (int layer in Enum.GetValues(typeof(Layer)))
                this.batches[layer] = new SpriteBatch(this.device);
            this.final = new SpriteBatch(this.device);

            //configure graphics parameters
            this.deviceManager.GraphicsProfile = GraphicsProfile.Reach;
            this.deviceManager.SynchronizeWithVerticalRetrace = true;
            this.deviceManager.HardwareModeSwitch = false;
            this.deviceManager.ApplyChanges();
        }

        public void BeginDraw(UIScreen screen, BlendState blend = null)
        {
            //lighting layer
            this.BatchOf(Layer.Glow).Begin(SpriteSortMode.Deferred, 
                BlendState.Additive, 
                SamplerState.AnisotropicClamp);

            //foreground and animated layer
            this.BatchOf(Layer.Fore).Begin(SpriteSortMode.Deferred, 
                blend ?? BlendState.NonPremultiplied, 
                SamplerState.PointClamp);

            //used for drawing cache
            this.final.Begin(SpriteSortMode.Deferred, 
                BlendState.Opaque, 
                SamplerState.PointClamp);

            //main layer (supports caching)
            if (screen is not UIMainScreen || UIMainScreen.Invalidated)
            {
                this.BatchOf(Layer.Main).Begin(SpriteSortMode.Deferred, 
                    blend ?? BlendState.NonPremultiplied, 
                    SamplerState.PointClamp);
                UIMainScreen.RefreshingCache = screen is UIMainScreen;
            } 
            this.DrawCalls = 0;
        }

        public void EndDraw(UIScreen screen)
        {
            if (screen is UIMainScreen)
            {
                if (UIMainScreen.RefreshingCache)
                {
                    //clear and re-render the cache texture
                    this.device.SetRenderTarget(UIMainScreen.RenderCache);
                    this.device.Clear(Tracker.Category is AllBlocks ? Config.Main.BorderColor : Config.Main.BackColor);
                    this.BatchOf(Layer.Main).End();
                    this.device.SetRenderTarget(null);
                }

                if (!Config.Main.HideRenderCache)
                    this.final.Draw(UIMainScreen.RenderCache, this.device.Viewport.Bounds, Color.White);
            }
            else
            {
                this.BatchOf(Layer.Main).End();
            }
            this.final.End();

            //render ambient glow effect
            if (Config.Main.ShowAmbientGlow && screen is not UIOverlayScreen)
                this.Draw("glow_ambient", this.device.Viewport.Bounds, this.RainbowStrong * 0.5f, Layer.Glow);
                
            this.BatchOf(Layer.Glow).End();
            this.BatchOf(Layer.Fore).End();
        }

        public void Update(Time time)
        {
            //fade rainbow to next color
            this.RainbowFast = ColorHelper.FromHSV(time.TotalFrames % 360, 0.5, 1.0);
            this.RainbowLight = ColorHelper.FromHSV(time.TotalFrames / 4 % 360, 0.33, 1.0);
            this.RainbowStrong = ColorHelper.FromHSV(time.TotalFrames / 8 % 360, 1.0, 1.0);
        }

        public void Draw(Texture2D texture, Rectangle destination, Color? tint = null, Layer layer = Layer.Main)
        {
            //draw a texture that isn't part of the main atlas
            if (texture is not null)
                this.BatchOf(layer).Draw(texture, destination, tint ?? Color.White);
            this.DrawCalls++;
        }

        public void Draw(string texture, Rectangle destination, Color? tint = null, Layer layer = Layer.Main)
        {
            if (string.IsNullOrWhiteSpace(texture))
                return;

            //if texture id not found, check for resolution specific variant of it
            SpriteSheet.TryGetRectangle(texture, out Rectangle source);
            if (source.IsEmpty)
                SpriteSheet.TryGetRectangle(texture + SpriteSheet.ResolutionPrefix + destination.Width, out source);
            this.BatchOf(layer).Draw(SpriteSheet.Atlas, destination, source, tint ?? Color.White);
            this.DrawCalls++;
        }

        public void Draw(string texture, Rectangle destination, Rectangle subSource, Color? tint = null, Layer layer = Layer.Main)
        {
            if (string.IsNullOrWhiteSpace(texture))
                return;

            //if texture id not found, check for resolution specific variant of it
            SpriteSheet.TryGetRectangle(texture, out Rectangle source);
            if (source.IsEmpty)
                SpriteSheet.TryGetRectangle(texture + SpriteSheet.ResolutionPrefix + destination.Width, out source);
            Rectangle finalSource = new (source.Location + subSource.Location, subSource.Size);
            this.BatchOf(layer).Draw(SpriteSheet.Atlas, destination, finalSource, tint ?? Color.White);
            this.DrawCalls++;
        }

        public void Draw(string texture, Vector2 center, float rotation, float scale = 1, Color? tint = null, Layer layer = Layer.Main)
        {
            this.Draw(texture, center, rotation, new Vector2(scale), tint, layer);
            this.DrawCalls++;
        }

        public void Draw(string texture, Vector2 center, float rotation, Vector2? scale = null, Color? tint = null, Layer layer = Layer.Main)
        {
            if (string.IsNullOrWhiteSpace(texture))
                return;
            SpriteSheet.TryGetRectangle(texture, out Rectangle source);
            Vector2 origin = new (source.Width / 2, source.Height / 2);
            this.BatchOf(layer).Draw(SpriteSheet.Atlas, center, source, tint ?? Color.White, rotation, origin, scale ?? Vector2.One, SpriteEffects.None, 0);
            this.DrawCalls++;
        }

        public void DrawRectangle(Rectangle rectangle, Color color, Color? borderColor = null, int border = 0, Layer layer = Layer.Main)
        {
            SpriteBatch batch = this.BatchOf(layer);
            borderColor ??= color;
            SpriteSheet.TryGetRectangle("pixel", out Rectangle source);
            if (border > 0 && borderColor != color)
            {
                //draw rectangle with border
                batch.Draw(SpriteSheet.Atlas, rectangle, source, borderColor.Value);
                var inner = new Rectangle(rectangle.X + border, rectangle.Y + border, rectangle.Width - border * 2, rectangle.Height - border * 2);
                batch.Draw(SpriteSheet.Atlas, inner, source, color);

                //draw rounded corners
                Color blended = ColorHelper.Blend(borderColor.Value, color, 0.5f);
                batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Left + border, rectangle.Top + border, 1, 1), source, blended);
                batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Right - (border + 1), rectangle.Top + border, 1, 1), source, blended);
                batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Right - (border + 1), rectangle.Bottom - (border + 1), 1, 1), source, blended);
                batch.Draw(SpriteSheet.Atlas, new Rectangle(rectangle.Left + border, rectangle.Bottom - (border + 1), 1, 1), source, blended);
                this.DrawCalls += 6;
            }
            else
            {
                batch.Draw(SpriteSheet.Atlas, rectangle, source, color);
                this.DrawCalls++;
            }
        }

        public void DrawString(DynamicSpriteFont font, string text, Vector2 location, Color color, Layer layer = Layer.Main)
        {
            this.BatchOf(layer).DrawString(font, text, location, color);
            this.DrawCalls++;
        }
    }
}

using System;
using AATool.Graphics;
using AATool.UI.Screens;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AATool.UI.Controls
{
    class UICoordinateGrid : UIControl
    {
        public const float NetherScale = 1f / 8;

        static readonly Color OverworldBackColor = Color.FromNonPremultiplied(255, 240, 200, 255);
        static readonly Color NetherBackColor = Color.FromNonPremultiplied(255, 128, 100, 255);

        static readonly Color OverworldLineColor = Color.FromNonPremultiplied(200, 220, 190, 255);
        static readonly Color NetherLineColor = Color.FromNonPremultiplied(200, 116, 110, 255);

        public bool IsNether { get; private set; }

        private Color backColor;
        private Color lineColor;

        protected float BlocksPerSquare = 512;
        protected float ViewScale = 64;
        protected float CoordScale = 1;

        protected Vector2 Offset = Vector2.Zero;
        protected Vector2 targetOffset = Vector2.Zero;

        public UICoordinateGrid()
        {
            this.backColor = Color.FromNonPremultiplied(255, 240, 200, 255);
            this.lineColor = Color.FromNonPremultiplied(200, 220, 190, 255);
        }

        protected override void UpdateThis(Time time)
        {
            //for testing coordinate transformation
            if (Input.Started(Keys.N))
                this.IsNether ^= true;

            this.UpdateOffset(time);

            float targetScale = this.IsNether ? NetherScale : 1;
            this.CoordScale = MathHelper.Lerp(this.CoordScale, targetScale, 16 * (float)time.Delta);

            Color targetBackColor = this.IsNether ? NetherBackColor : OverworldBackColor;
            int r = (int)MathHelper.Lerp(this.backColor.R, targetBackColor.R, 4 * (float)time.Delta);
            int g = (int)MathHelper.Lerp(this.backColor.G, targetBackColor.G, 4 * (float)time.Delta);
            int b = (int)MathHelper.Lerp(this.backColor.B, targetBackColor.B, 4 * (float)time.Delta);
            this.backColor = Color.FromNonPremultiplied(r, g, b, 255);

            Color targetLineColor = this.IsNether ? NetherLineColor : OverworldLineColor;
            r = (int)MathHelper.Lerp(this.lineColor.R, targetLineColor.R, 4 * (float)time.Delta);
            g = (int)MathHelper.Lerp(this.lineColor.G, targetLineColor.G, 4 * (float)time.Delta);
            b = (int)MathHelper.Lerp(this.lineColor.B, targetLineColor.B, 4 * (float)time.Delta);
            this.lineColor = Color.FromNonPremultiplied(r, g, b, 255);
        }

        private void UpdateOffset(Time time)
        {
            if (Input.IsDown(Keys.A))
                this.targetOffset -= new Vector2(10, 0);
            if (Input.IsDown(Keys.D))
                this.targetOffset += new Vector2(10, 0);
            if (Input.IsDown(Keys.W))
                this.targetOffset -= new Vector2(0, 10);
            if (Input.IsDown(Keys.S))
                this.targetOffset += new Vector2(0, 10);

            float lerpAmount = 15 * (float)time.Delta;
            float x = MathHelper.Lerp(this.Offset.X, this.targetOffset.X, lerpAmount);
            float y = MathHelper.Lerp(this.Offset.Y, this.targetOffset.Y, lerpAmount);
            this.Offset = new Vector2(x, y);
        }

        public void Recenter()
        {
            this.targetOffset = Vector2.Zero;
        }

        public override void ResizeThis(Rectangle parentRectangle)
        {
            base.ResizeThis(parentRectangle);
        }

        public override void DrawThis(Canvas canvas)
        {
            canvas.DrawRectangle(this.Inner, Color.DarkViolet, Color.Magenta, 1, Layer.Fore);
            this.DrawGrid(canvas);
            this.DrawEdges(canvas);
        }

        private void DrawGrid(Canvas canvas)
        {
            //display.DrawRectangle(new Rectangle(ContentRectangle.X - 24, ContentRectangle.Y - 24, ContentRectangle.Width + 48, ContentRectangle.Height + 48), backColor);
            canvas.DrawRectangle(this.Inner, this.backColor, this.lineColor, 1, Layer.Fore);

            float cellSize = Math.Abs(this.BlocksPerSquare / this.ViewScale / this.CoordScale);
            int wrappedX = (int)Math.Round(this.Offset.X % cellSize);
            int wrappedY = (int)Math.Round(this.Offset.Y % cellSize);
            var wrappedOffset = new Point(wrappedX, wrappedY);

            //vertical
            for (float x = -cellSize; x < Inner.Width / 2; x += cellSize)
            {
                //if (x < this.Left || x > this.Right)
                //    break;
                canvas.DrawRectangle(new Rectangle(Inner.Center.X - wrappedOffset.X - (int)Math.Round(x), Inner.Top, 1, Height), lineColor, null, 0, Layer.Fore);
                canvas.DrawRectangle(new Rectangle(Inner.Center.X - wrappedOffset.X + (int)Math.Round(x), Inner.Top, 1, Height), lineColor, null, 0, Layer.Fore);
            }
            //horizontal
            for (float y = -cellSize; y < Inner.Height / 2; y += cellSize)
            {
                //if (y < this.Top || y > this.Bottom)
                //    break;
                canvas.DrawRectangle(new Rectangle(Inner.Left, Inner.Center.Y - wrappedOffset.Y - (int)Math.Round(y), Width, 1), lineColor, null, 0, Layer.Fore);
                canvas.DrawRectangle(new Rectangle(Inner.Left, Inner.Center.Y - wrappedOffset.Y + (int)Math.Round(y), Width, 1), lineColor, null, 0, Layer.Fore);
            }

            //origin lines
            canvas.DrawRectangle(new Rectangle(Inner.Center.X - (int)Math.Round(Offset.X), Inner.Top, 1, Height), Color.Black * 0.25f, null, 0, Layer.Fore);
            canvas.DrawRectangle(new Rectangle(Inner.Left, Inner.Center.Y - (int)Math.Round(Offset.Y), Width, 1), Color.Black * 0.25f, null, 0, Layer.Fore);
        }

        private void DrawEdges(Canvas canvas)
        {
            int size = 25;
            //top
            canvas.DrawRectangle(new Rectangle(Left, Top, Width, size + 1), lineColor, null, 0, Layer.Fore);
            //bottom
            canvas.DrawRectangle(new Rectangle(Left, Bottom - size, Width, size), lineColor, null, 0, Layer.Fore);
            //left
            canvas.DrawRectangle(new Rectangle(Left, Top, size + 1, Height), lineColor, null, 0, Layer.Fore);
            //right
            canvas.DrawRectangle(new Rectangle(Right - size, Top, size, Height), lineColor, null, 0, Layer.Fore);
            //left border
            canvas.DrawRectangle(new Rectangle(Left, Top, 1, Height), Color.FromNonPremultiplied(130, 135, 144, 255), null, 0, Layer.Fore);

            //legend
            //canvas.DrawStringCentered(font, "1 Square = " + BlocksPerSquare + " Blocks", new Vector2(Center.X, Bottom - size / 2), Color.Black);

            //text
            int gridSize = ((Width - size * 2) / 500) * 500;
            int labelCount = gridSize / 250;
            for (int i = -labelCount / 2; i <= labelCount / 2; i++)
            {
                int position = (int)(gridSize / (double)labelCount * i);
                //display.DrawStringCentered(font, Math.Round(position * viewScale * coordScale + offset.X * viewScale * coordScale).ToString(), new Vector2(Center.X + position, Top + size / 2), Color.Black);
            }
        }
    }
}

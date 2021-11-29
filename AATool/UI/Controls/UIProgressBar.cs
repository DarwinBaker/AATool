using AATool;
using AATool.Graphics;
using AATool.UI.Controls;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIProgressBar : UIControl
    {
        private const int SEGMENT_WIDTH  = 20;
        private const int SEGMENT_HEIGHT = 10;

        public float Value         { get; private set; }
        public float Min           { get; private set; }
        public float Max           { get; private set; }
        public float Range         { get; private set; }
        public float Percent       { get; private set; }
        public float DisplayValue  { get; private set; }

        private void ClampValue()    => this.LerpToValue(this.Value);
        private void UpdateRange()   => this.Range = Math.Abs(this.Min - this.Max);
        private void UpdatePercent() => this.Percent = (this.Value - this.Min) / (this.Max - this.Min);

        public void LerpToValue(float value)
        {
            value = Math.Min(Math.Max(value, this.Min), this.Max);
            if (this.Value == value)
                return;

            this.Value = value;
            this.UpdatePercent();
        }

        public void SkipToValue(float value)
        {
            this.Value = Math.Min(Math.Max(value, this.Min), this.Max);
            this.DisplayValue = this.Value;
            this.UpdatePercent();
        }

        public void SetMin(float value)
        {
            this.Min = value;
            this.ClampValue();
            this.UpdateRange();
            this.UpdatePercent();
        }

        public void SetMax(float value)
        {
            this.Max = value;
            this.ClampValue();
            this.UpdateRange();
            this.UpdatePercent();
        }

        protected override void UpdateThis(Time time)
        {
            if (this.DisplayValue == this.Value)
                return;

            float smoothingFactor = this.Width * (float)time.Delta * 0.8f;
            if (this.DisplayValue > this.Value)
                this.DisplayValue = Math.Max((float)(this.DisplayValue - smoothingFactor), (float)this.Value);
            else if (this.DisplayValue < this.Value)
                this.DisplayValue = Math.Min((float)(this.DisplayValue + smoothingFactor), (float)this.Value);
        }

        public override void ResizeThis(Rectangle parentRectangle)
        {
            //scale all relative values to parent rectangle
            this.Margin.Resize(parentRectangle.Size);
            this.FlexWidth.Resize(parentRectangle.Width);
            this.FlexHeight.Resize(parentRectangle.Height);
            this.MaxWidth.Resize(parentRectangle.Width);
            this.MinWidth.Resize(parentRectangle.Width);
            this.MaxHeight.Resize(parentRectangle.Height);
            this.MinHeight.Resize(parentRectangle.Height);

            //clamp size to min and max
            this.Width  = MathHelper.Clamp(this.FlexWidth.Absolute / SEGMENT_WIDTH, MinWidth.Absolute, MaxWidth.Absolute) * SEGMENT_WIDTH;
            this.Height = MathHelper.Clamp(this.FlexHeight.Absolute, MinHeight.Absolute, MaxHeight.Absolute);

            //if control is square, conform both width and height to the larger of the two
            if (this.IsSquare)
                this.Width = this.Height = Math.Min(this.Width, this.Height);

            this.X = this.HorizontalAlign switch
            {
                HorizontalAlign.Center => parentRectangle.X + (parentRectangle.Width / 2) - (this.Width / 2),
                HorizontalAlign.Left => parentRectangle.Left + this.Margin.Left,
                _ => parentRectangle.Right - this.Width - this.Margin.Right
            };

            this.Y = this.VerticalAlign switch
            {
                VerticalAlign.Center => parentRectangle.Top + (parentRectangle.Height / 2) - (this.Height / 2),
                VerticalAlign.Top => parentRectangle.Top + this.Margin.Top,
                _ => parentRectangle.Bottom - this.Height - this.Margin.Bottom
            };

            this.Padding.Resize(this.Size);

            //calculate internal rectangle
            this.Content = new Rectangle(this.X + this.Margin.Left + this.Padding.Left, Y + Margin.Top + Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);
        }

        public override void DrawThis(Display display)
        {
            if (this.SkipDraw)
                return;

            this.DrawSegments(display, "inactive", this.Bounds);
            this.DrawSegments(display, "active", new Rectangle(X, Y, (int)Math.Round(Width * (DisplayValue - Min) / (Max - Min)), Height));
        }

        private void DrawSegments(Display display, string texture, Rectangle rectangle)
        {
            //draw left, center, and right segments
            string segmentTex = "bar_" + texture + "_left";
            int lastSegment = rectangle.Width / SEGMENT_WIDTH;
            for (int i = 0; i < lastSegment; i++)
            {
                if (i == 1)
                    segmentTex = "bar_" + texture + "_middle";
                else if (i == (Width / SEGMENT_WIDTH) - 1)
                    segmentTex = "bar_" + texture + "_right";

                var segmentRect = new Rectangle(rectangle.Left + SEGMENT_WIDTH * i, rectangle.Top, SEGMENT_WIDTH, SEGMENT_HEIGHT);
                display.Draw(segmentTex, segmentRect, Color.White);
            }
        }

        public override void DrawDebugRecursive(Display display)
        {
            if (this.IsCollapsed)
                return;

            //fill
            display.DrawRectangle(this.Bounds, ColorHelper.Fade(this.DebugColor, 0.2f), null, 0, Layer.Fore);

            //edges
            display.DrawRectangle(new Rectangle(this.Bounds.Left, this.Bounds.Top, this.Bounds.Width, 1),
                ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
            display.DrawRectangle(new Rectangle(this.Bounds.Right - 1, this.Bounds.Top + 1, 1, this.Bounds.Height - 2),
                ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
            display.DrawRectangle(new Rectangle(this.Bounds.Left + 1, this.Bounds.Bottom - 1, this.Bounds.Width - 1, 1),
                ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
            display.DrawRectangle(new Rectangle(this.Bounds.Left, this.Bounds.Top + 1, 1, this.Bounds.Height - 1),
                ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
            
            for (int i = 0; i < this.Children.Count; i++)
                this.Children[i].DrawDebugRecursive(display);
        }


        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.FlexHeight = new Size(SEGMENT_HEIGHT, SizeMode.Absolute);
        }
    }
}

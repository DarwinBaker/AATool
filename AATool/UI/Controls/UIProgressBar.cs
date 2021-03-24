using AATool;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIProgressBar : UIControl
    {
        private const int SEGMENT_WIDTH = 20;
        private const int SEGMENT_HEIGHT = 10;

        public double Value     { get; private set; }
        public double Min       { get; private set; }
        public double Max       { get; private set; }
        public double Range     { get; private set; }
        public double Percent   { get; private set; }

        public double DisplayValue { get; private set; }

        protected float lerpSpeed = 20;
        protected Color backColor = Color.DarkGreen;
        protected Color foreColor = Color.Lime;

        public void ClampValue() => SetValue(Value);
        public void UpdateRange() => Range = Math.Abs(Min - Max);
        public void UpdatePercent() => Percent = (Value - Min) / (Max - Min);

        public void SetValue(double value)
        {
            value = Math.Min(Math.Max(value, Min), Max);
            if (Value == value)
                return;
            Value = value;
            UpdatePercent();
        }

        public void SetMin(double value)
        {
            Min = value;
            ClampValue();
            UpdateRange();
            UpdatePercent();
        }

        public void SetMax(double value)
        {
            Max = value;
            ClampValue();
            UpdateRange();
            UpdatePercent();
        }

        protected override void UpdateThis(Time time)
        {
            DisplayValue = MathHelper.Lerp((float)DisplayValue, (float)Value, lerpSpeed * (float)time.Delta);
            base.UpdateThis(time);
        }

        public override void ResizeThis(Rectangle parentRectangle)
        {
            //scale all relative values to parent rectangle
            Margin.Resize(parentRectangle.Size);
            FlexWidth.Resize(parentRectangle.Width);
            FlexHeight.Resize(parentRectangle.Height);
            MaxWidth.Resize(parentRectangle.Width);
            MinWidth.Resize(parentRectangle.Width);
            MaxHeight.Resize(parentRectangle.Height);
            MinHeight.Resize(parentRectangle.Height);

            //clamp size to min and max
            Width  = MathHelper.Clamp(FlexWidth.Absolute / SEGMENT_WIDTH, MinWidth.Absolute, MaxWidth.Absolute) * SEGMENT_WIDTH;
            Height = MathHelper.Clamp(FlexHeight.Absolute, MinHeight.Absolute, MaxHeight.Absolute);

            //if control is square, conform both width and height to the larger of the two
            if (IsSquare) Width = Height = Math.Min(Width, Height);

            X = HorizontalAlign switch
            {
                HorizontalAlign.Center => (parentRectangle.X + (parentRectangle.Width / 2) - (Width / 2)),
                HorizontalAlign.Left => parentRectangle.Left + Margin.Left,
                _ => parentRectangle.Right - Width - Margin.Right
            };

            Y = VerticalAlign switch
            {
                VerticalAlign.Center => (parentRectangle.Top + parentRectangle.Height / 2 - Height / 2),
                VerticalAlign.Top => parentRectangle.Top + Margin.Top,
                _ => parentRectangle.Bottom - Height - Margin.Bottom
            };

            Padding.Resize(Size);

            //calculate internal rectangle
            ContentRectangle = new Rectangle(X + Margin.Left + Padding.Left, Y + Margin.Top + Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);
        }

        public override void DrawThis(Display display)
        {
            DrawSegments(display, "inactive", Rectangle);
            DrawSegments(display, "active", new Rectangle(X, Y, (int)Math.Round(Width * (DisplayValue - Min) / (Max - Min)), Height));
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

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            FlexHeight = new Size(SEGMENT_HEIGHT, SizeMode.Absolute);
        }
    }
}

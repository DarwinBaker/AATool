using AATool.Configuration;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIProgressBar : UIControl
    {
        private const int SegmentWidth = 20;
        private const int SegmentHeight = 10;

        public float Value   { get; private set; }
        public float Min     { get; private set; }
        public float Max     { get; private set; }
        public float Range   { get; private set; }
        public float Percent { get; private set; }
        public float Lerped  { get; private set; }

        private void ClampValue()    => this.StartLerpToValue(this.Value);
        private void UpdateRange()   => this.Range = Math.Abs(this.Min - this.Max);
        private void UpdatePercent() => this.Percent = (this.Value - this.Min) / (this.Max - this.Min);

        private Rectangle leftSrc;
        private Rectangle rightSrc;

        private Rectangle emptyLeft;
        private Rectangle emptyMiddle;
        private Rectangle emptyRight;

        private Rectangle filledLeft;
        private Rectangle filledMiddle;
        private Rectangle filledRight;
        private Rectangle filledBounds;
        private Rectangle edgeDest;
        private Rectangle glowDest;

        private Color glowColor;

        private string style;

        public UIProgressBar()
        {
            this.UpdateStyle();
        }

        public void StartLerpToValue(float value)
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
            this.Lerped = this.Value;
            this.UpdatePercent();
            this.UpdateGlowColor();
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
            if (Config.Main.ProgressBarStyle.Changed)
                this.UpdateStyle();

            bool refresh = Config.Main.ProgressBarStyle.Changed 
                || Math.Round(this.Lerped, 4) != Math.Round(this.Value, 4);

            if (refresh)
            {
                this.Lerped = MathHelper.Lerp(this.Lerped, this.Value, (float)time.Delta * 16f);
                this.UpdateFill();
                this.UpdateGlowColor();
                UIMainScreen.Invalidate();
            }
        }

        private void UpdateStyle()
        {
            this.style = Config.Main.ProgressBarStyle.Value.ToLower().Replace(" ", "_");
            this.UpdateGlowColor();
        }

        private void UpdateFill()
        {
            int full = (int)Math.Round(this.Width * (this.Lerped / this.Max), MidpointRounding.AwayFromZero);

            this.filledBounds = new Rectangle(
                this.X,
                this.Y, 
                (int)Math.Round(this.Width * (this.Lerped - this.Min) / (this.Max - this.Min)),
                this.Height);

            int leftWidth = Math.Min(full, 20);
            int rightWidth = Math.Max(full - (this.Width - 20), 0);

            this.leftSrc = new Rectangle(0, 0, leftWidth, 10);
            this.filledLeft = new Rectangle(this.Left, this.Top, leftWidth, 10);

            this.rightSrc = new Rectangle(0, 0, rightWidth, 10);
            this.filledRight = new Rectangle(this.Right - 20, this.Top, rightWidth, 10);

            this.filledMiddle = new Rectangle(
                this.Left + this.filledLeft.Width,
                this.Top,
                full - 20 - rightWidth,
                10);

            this.edgeDest = new Rectangle(
                Math.Min(this.Left + full, this.filledRight.Left),
                this.Top,
                Math.Min(full - 5, 5),
                10);

            this.glowDest = new Rectangle(
                this.Left - 6,
                this.Top - 5,
                full + 12,
                this.Height + 10);
        }

        private void UpdateGlowColor()
        {
            this.glowColor = this.style switch {
                "modern"       => Color.White * Math.Min(this.Percent * 1.25f, 0.7f),
                "experience"   => Color.Green * Math.Min(this.Percent * 1.25f, 0.7f),
                "ender_dragon" => Color.Magenta * Math.Min(this.Percent * 1.25f, 0.7f),
                _ => Color.White
            } ;
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
            this.Width = MathHelper.Clamp(this.FlexWidth, this.MinWidth, this.MaxWidth - this.Margin.Horizontal);
            this.Height = MathHelper.Clamp(this.FlexHeight, this.MinHeight, this.MaxHeight - this.Margin.Vertical);

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
            this.Padding = new Margin(2, 2, 2, 2);
            this.Padding.Resize(this.Size);
            
            //calculate internal rectangle
            this.Inner = new Rectangle(this.X + this.Padding.Left,
                this.Y + this.Padding.Top,
                this.Width - this.Padding.Horizontal,
                this.Height - this.Padding.Vertical);

            this.emptyLeft = new Rectangle(this.Left, this.Top, 20, 10);
            this.emptyMiddle = new Rectangle(this.Left + 20, this.Top, this.Width - 40, 10);
            this.emptyRight = new Rectangle(this.Right - 20, this.Top, 20, 10);

            this.UpdateFill();
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.style is "none")
                return;

            canvas.Draw("progress_bar_glow", this.glowDest, this.glowColor, Layer.Glow);
            if (this.style is "experience")
            {
                this.DrawSegmented(canvas, $"{this.style}_inactive", this.Bounds);
                this.DrawSegmented(canvas, $"{this.style}_active", this.filledBounds);
            }
            else
            {
                this.DrawSmooth(canvas);
            }
        }

        private void DrawSmooth(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            //empty background
            canvas.DrawRectangle(this.Inner, Config.Main.BackColor.Value * 0.5f);

            //empty foreground
            Color tint = this.style is "modern" or "" ? Config.Main.BorderColor : Color.White;
            canvas.Draw($"bar_{this.style}_inactive_left", this.emptyLeft, tint);
            canvas.Draw($"bar_{this.style}_inactive_middle", this.emptyMiddle, tint);
            canvas.Draw($"bar_{this.style}_inactive_right", this.emptyRight, tint);

            Color faded = ColorHelper.Fade(Color.White, 0.1f);
            canvas.Draw($"bar_{this.style}_active_left", this.emptyLeft, faded);
            canvas.Draw($"bar_{this.style}_active_middle", this.emptyMiddle, faded);
            canvas.Draw($"bar_{this.style}_active_right", this.emptyRight, faded);

            //filled foreground
            canvas.Draw($"bar_{this.style}_active_left", this.filledLeft, this.leftSrc, Color.White);
            canvas.Draw($"bar_{this.style}_active_middle", this.filledMiddle, Color.White);
            canvas.Draw($"bar_{this.style}_active_right", this.filledRight, this.rightSrc, Color.White);
            canvas.Draw($"bar_{this.style}_active_edge", this.edgeDest, Color.White);
        }

        private void DrawSegmented(Canvas canvas, string texture, Rectangle rectangle)
        {
            if (this.SkipDraw)
                return;

            //draw left, center, and right segments
            string segmentTex = "bar_" + texture + "_left";
            int lastSegment = rectangle.Width / SegmentWidth;
            for (int i = 0; i < lastSegment; i++)
            {
                if (i is 1)
                    segmentTex = "bar_" + texture + "_middle";
                else if (i == (this.Width / SegmentWidth) - 1)
                    segmentTex = "bar_" + texture + "_right";

                var segmentRect = new Rectangle(
                    rectangle.Left + (SegmentWidth * i), 
                    rectangle.Top, 
                    SegmentWidth, 
                    SegmentHeight);

                canvas.Draw(segmentTex, segmentRect, Color.White);
            }
        }

        public override void DrawDebugRecursive(Canvas canvas)
        {
            if (this.IsCollapsed)
                return;

            //fill
            canvas.DrawRectangle(this.Bounds, ColorHelper.Fade(this.DebugColor, 0.2f), null, 0, Layer.Fore);

            //edges
            canvas.DrawRectangle(new Rectangle(this.Bounds.Left, this.Bounds.Top, this.Bounds.Width, 1),
                ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
            canvas.DrawRectangle(new Rectangle(this.Bounds.Right - 1, this.Bounds.Top + 1, 1, this.Bounds.Height - 2),
                ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
            canvas.DrawRectangle(new Rectangle(this.Bounds.Left + 1, this.Bounds.Bottom - 1, this.Bounds.Width - 1, 1),
                ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
            canvas.DrawRectangle(new Rectangle(this.Bounds.Left, this.Bounds.Top + 1, 1, this.Bounds.Height - 1),
                ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
            
            for (int i = 0; i < this.Children.Count; i++)
                this.Children[i].DrawDebugRecursive(canvas);
        }


        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.FlexHeight = new Size(SegmentHeight, SizeMode.Absolute);
        }
    }
}

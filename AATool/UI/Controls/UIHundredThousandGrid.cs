using System;
using AATool.Configuration;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    internal class UIHundredThousandGrid : UIPanel
    {
        const int Total = 100000;
        const int DotSize = 2;
        const int DotPadding = 1;

        Rectangle lastDot;
        bool resized;
        float arrowAlpha = 0;

        Run data;

        UITextBlock labelPercent;
        UITextBlock labelProgress;
        UITextBlock labelRemaining;

        private string Runner => this.data?.Runner ?? string.Empty;
        private int Completed => this.data?.ExtraStat ?? 0;
        private TimeSpan AverageIgt => this.data?.InGameTime ?? default;

        public UIHundredThousandGrid() : base()
        {
            this.BuildFromTemplate();
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.TryGetFirst(out this.labelPercent, "percent");
            this.TryGetFirst(out this.labelProgress, "progress");
            this.TryGetFirst(out this.labelRemaining, "remaining");
        }

        public void SetData(Run data)
        {
            this.data = data;
            this.UpdateLabels();
        }

        private void UpdateLabels()
        {
            this.labelProgress?.SetText($"{this.Completed:N0} of {Total:N0}");
            this.labelPercent?.SetText($"{(float)this.Completed / Total * 100:0.00}%");

            int remainingRuns = Total - this.Completed;
            var remainingTime = TimeSpan.FromMinutes(remainingRuns * this.AverageIgt.TotalMinutes);
            int minutesPerRun = (int)Math.Round(this.AverageIgt.TotalMinutes);
            int remainingHours = (int)Math.Round(remainingTime.TotalHours);

            if (remainingHours > 0)
            {
                this.labelRemaining?.SetText($"{remainingHours:N0}hrs @ {minutesPerRun}min/run");
            }
            else
            {
                this.labelRemaining?.SetText($"All Done, Congratulations!!!");
            }
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.resized && this.DrawMode == DrawMode.All && this.Parent?.Parent is UIGrid grid)
            {
                grid.SetColWidth(1, new Size(190));
                grid.SetColWidth(3, new Size(1390));
                grid.ResizeRecursive(grid.Parent.Inner);
                this.resized = true;
            }

            this.arrowAlpha = MathHelper.Lerp(this.arrowAlpha, 1.0f, (float)(3.0f * time.Delta));
        }

        public override void DrawThis(Canvas canvas)
        {
            if (!this.SkipDraw)
            {
                base.DrawThis(canvas);
                this.DrawGrid(canvas);
            }

            this.DrawArrow(canvas);
        }

        private void DrawGrid(Canvas canvas)
        {
            int cols = 500;
            int rows = Total / cols;

            int left = this.Inner.Left;
            int bottom = this.Inner.Bottom - 2;

            int filled = 0;

            Color filledColor = Config.Main.TextColor;
            Color emptyColor = Config.Main.TextColor.Value * 0.4f;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < cols; column++)
                {
                    int x = left + (column * DotSize) + (DotPadding * column);
                    int y = bottom - (row * DotSize) - (DotPadding * row);
                    var bounds = new Rectangle(x, y, DotSize, DotSize);

                    Color color = filled < this.Completed ? filledColor : emptyColor;

                    canvas.Draw("grid_1", bounds, color);
                    filled++;

                    if (filled == this.Completed)
                        this.lastDot = bounds;
                }
            }
        }

        private void DrawArrow(Canvas canvas)
        {
            if (this.Completed >= 100000)
                return;

            const int Width = 28;
            const int Height = 33;
            var arrowBounds = new Rectangle(
                this.lastDot.Center.X - (Width / 2),
                this.lastDot.Top - Height - 2,
                Width,
                Height);

            const int AvatarSize = 16;
            var avatarBounds = new Rectangle(
                arrowBounds.Center.X - (AvatarSize / 2),
                arrowBounds.Center.Y - (AvatarSize / 2) - 2,
                AvatarSize,
                AvatarSize);

            Color arrowColor = ColorHelper.Fade(Config.Main.TextColor, this.arrowAlpha);
            Color avatarColor = ColorHelper.Fade(Color.White, this.arrowAlpha);

            canvas.Draw("100k_arrow", arrowBounds, arrowColor, Layer.Fore);
            canvas.Draw($"avatar-{this.Runner.ToLower()}", avatarBounds, avatarColor, Layer.Fore);
        }
    }
}

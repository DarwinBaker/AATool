using System;
using System.Collections.Generic;
using AATool.Configuration;
using AATool.Graphics;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIDayNightCycle : UIControl
    {
        public TimeSpan TargetStart { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Length { get; set; } = TimeSpan.FromMinutes(60);

        public float PixelsPerMinute => (float)(this.Width / this.Length.TotalMinutes);

        private Dictionary<TimeSpan, Rectangle> dayBlocks = new ();
        private Dictionary<TimeSpan, Rectangle> nightBlocks = new ();
        private bool initialized = false;

        private int ToClientX(TimeSpan time)
        {
            double minutesAfterStart = time.TotalMinutes - Start.TotalMinutes;
            int scaled = (int)((minutesAfterStart / Length.TotalMinutes) * this.Width);
            scaled = Math.Max(scaled, 0);
            scaled = Math.Min(scaled, this.Width);
            return this.Left + scaled;
        }

        public override void ResizeThis(Rectangle parent)
        {
            base.ResizeThis(parent);
            this.Refresh();
        }

        protected override void UpdateThis(Time time)
        {
            bool invalidated = Tracker.Invalidated || Tracker.SavesFolderChanged || Tracker.WorldChanged;
            if (invalidated)
                this.TargetStart = Tracker.InGameTime;

            if (!initialized || Tracker.SavesFolderChanged || Tracker.WorldChanged)
            {
                this.Start = this.TargetStart;
                initialized = true;
            }

            float amount = (float)(15 * time.Delta);
            float lerpedSeconds = MathHelper.Lerp((float)Start.TotalSeconds, 
                (float)TargetStart.TotalSeconds, amount);

            bool changed = (int)Start.TotalSeconds != (int)lerpedSeconds;
            Start = TimeSpan.FromSeconds(lerpedSeconds);
            if (changed || invalidated)
                Refresh();
        }

        public void Refresh()
        {
            this.dayBlocks.Clear();
            this.nightBlocks.Clear();

            const int MinecraftDaySeconds = 60 * 10;
            TimeSpan end = this.Start.Add(this.Length);
            TimeSpan cursor = this.Start;

            int blockStartX = this.ToClientX(this.Start);
            int secondsRemaining = MinecraftDaySeconds - ((int)this.Start.TotalSeconds % MinecraftDaySeconds);

            while (cursor < end)
            {
                bool isDay = MoonPhase.IsDayTime(cursor);
                TimeSpan prevCursor = cursor;
                cursor = cursor.Add(TimeSpan.FromSeconds(secondsRemaining));

                int blockEndX = this.ToClientX(cursor);
                int blockWidth = blockEndX - blockStartX;

                var block = new Rectangle(
                    blockStartX, this.Top,
                    blockWidth, this.Height);
                (isDay ? this.dayBlocks : this.nightBlocks).Add(prevCursor, block);

                secondsRemaining = MinecraftDaySeconds;
                blockStartX = blockEndX;
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            this.DrawNights(canvas);
            this.DrawDays(canvas);

            if (!this.SkipDraw)
                this.DrawLabels(canvas);
        }

        private void DrawNights(Canvas canvas)
        {
            foreach (KeyValuePair<TimeSpan, Rectangle> night in this.nightBlocks)
            {
                Rectangle block = night.Value;
                canvas.DrawRectangle(block, Color.Blue, null, 0, Layer.Fore);

                var moonBounds = new Rectangle(
                    block.Center.X - 8,
                    this.Bottom + 4,
                    16,
                    16);
                
                int phase = (int)MoonPhase.PhaseOf(night.Key);
                canvas.Draw($"moon_{phase}", moonBounds, Color.White, Layer.Fore);
            }
        }

        private void DrawDays(Canvas canvas)
        {
            foreach (KeyValuePair<TimeSpan, Rectangle> day in this.dayBlocks)
            {
                canvas.DrawRectangle(day.Value, Color.Yellow, null, 0, Layer.Fore);
            }
        }

        private void DrawLabels(Canvas canvas)
        {
            const int height = 8;
            var lineLeft = new Rectangle(this.Left + 1, this.Top - height - 3, 1, height);
            var lineRight = new Rectangle(this.Right - 1, this.Top - height - 3, 1, height);
            var lineCenter = new Rectangle(this.Center.X, this.Top - height - 3, 1, height);

            canvas.DrawRectangle(lineLeft, Config.Main.TextColor);
            canvas.DrawRectangle(lineRight, Config.Main.TextColor);
            canvas.DrawRectangle(lineCenter, Config.Main.TextColor);
        }
    }
}

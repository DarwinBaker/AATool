using System;
using System.Diagnostics;
using AATool.Configuration;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIPersonalBestHardcoreChallenge : UIPersonalBest
    {
        const int DotSize = 8;
        const int DotPadding = 1;
        const int DotColumns = 20;

        private protected UITextBlock best;
        private protected UITextBlock deaths;
        private protected UITextBlock runs;

        HardcoreStreak streak;

        public UIPersonalBestHardcoreChallenge(Leaderboard owner) : base(owner)
        {
        }

        public override void SetRun(Run run, bool isClaimed = true)
        {
            if (run is HardcoreStreak streak)
            {
                base.SetRun(run, isClaimed);
                this.streak = streak;
                int completed = streak.BestStreak;
                int dotRows = (int)Math.Ceiling((float)completed / DotColumns);
                dotRows = Math.Max(dotRows, 100 / DotColumns);
                int dotGridHeight = (dotRows * DotSize) + (dotRows * DotPadding);
                this.FlexHeight = new Size(76 + dotGridHeight, SizeMode.Absolute);
            }
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);

            this.face.LockBadgeAndFrame = true;

            this.best = this.First<UITextBlock>("best");
            this.deaths = this.First<UITextBlock>("deaths");
            this.runs = this.First<UITextBlock>("runs");

            this.best.SetText($"{this.streak.BestStreak} / 100");

            if (this.streak.Date == default)
            {
                this.date.SetText(string.Empty);
                this.age.SetText(string.Empty);
            }

            if (this.streak.InGameTime == default)
            {
                this.igt.SetText("---");
                //this.igt.Collapse();
                //this.First("igt_label")?.Collapse();
                //this.First("igt_splitter")?.Collapse();
            }

            if (int.TryParse(this.streak.Runs, out _))
            {
                this.runs.SetText(this.streak.Runs.ToString());
            }
            else
            {
                this.runs.SetText("---");
            }

            this.deaths.SetText(this.streak.Deaths.ToString());

            this.igt.Margin = new Margin(0, 0, 0, 0);
            this.date.Padding = new Margin(0, 0, 0, 0);
            this.age.Padding = new Margin(0, 0, 0, 0);
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.face.SetBadge(new HundredHardcoreBadge(this.streak.BestStreak));
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            base.DrawThis(canvas);
            this.DrawGrid(canvas);
        }

        private void DrawGrid(Canvas canvas)
        {
            int left = this.Inner.Left + 16;
            int bottom = this.Inner.Bottom - 28;

            int completed = this.streak.BestStreak;
            int rows = (int)Math.Ceiling((float)completed / DotColumns);
            rows = Math.Max(rows, 100 / DotColumns);
            int drawn = 0;


            Color died = new Color(200, 32, 32);
            Color filled = Config.Main.TextColor;
            Color empty = Config.Main.TextColor.Value * 0.4f;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < DotColumns; column++)
                {
                    int x = left + (column * DotSize) + (DotPadding * column);
                    int y = bottom - (row * DotSize) - (DotPadding * row);
                    var bounds = new Rectangle(x, y, DotSize, DotSize);

                    Color color;
                    if (drawn == completed && !this.streak.OnBestStreak)
                    {
                        color = died;
                    }
                    else if (drawn < 100)
                    {
                        color = drawn < completed ? filled : empty;
                    }
                    else
                    {
                        color = drawn < completed ? filled : Color.Transparent;
                    }

                    canvas.Draw("hc_completion", bounds, color);
                    drawn++;
                }
            }
        }
    }
}

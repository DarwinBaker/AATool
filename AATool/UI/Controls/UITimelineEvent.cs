using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UITimelineEvent : UIControl
    {
        const int BarThickness = 2;

        private Objective objective;
        private UIPicture icon;
        private UITextBlock label;
        private UIGlowEffect glow;
        private Rectangle barRectangle;
        private DateTime start;

        private readonly string timestamp;
        private readonly bool top;

        public UITimelineEvent(Objective objective, DateTime start, bool top)
        {
            this.objective = objective;
            this.start = start;
            this.top = top;
            if (this.objective is not null)
            {
                TimeSpan igt = objective.WhenFirstCompleted.Subtract(this.start);
                this.timestamp = igt.TotalDays > 1
                    ? $"{(int)igt.TotalHours}:{igt:mm':'ss}"
                    : igt.ToString("hh':'mm':'ss");
            }
            this.BuildFromTemplate();
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.icon  = this.First<UIPicture>("icon");
            this.glow  = this.First<UIGlowEffect>();
            this.label = this.First<UITextBlock>("label");

            this.icon.SetTexture(this.objective?.Icon);
            this.icon.SetLayer(Layer.Fore);

            this.label.SetText(this.timestamp);
        }

        public override void ResizeThis(Rectangle parent)
        {
            base.ResizeThis(parent);

            int height = 56;
            int y = this.top ? this.Bottom : this.Top - height;
            this.barRectangle = new(
                this.Bounds.Center.X - (BarThickness / 2),
                y,
                BarThickness,
                height);
        }

        public override void DrawThis(Canvas canvas)
        {
            canvas.DrawRectangle(this.barRectangle, Config.Main.TextColor);
        }
    }
}

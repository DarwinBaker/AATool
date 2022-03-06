using System;
using AATool.Data;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Screens;

namespace AATool.UI.Controls
{
    public class UIPersonalBest : UIPanel
    {
        public PersonalBest Run { get; set; }

        private UIAvatar face;

        public UIPersonalBest(PersonalBest pb)
        {
            this.Run = pb;
            this.BuildFromTemplate();
        }

        protected override void UpdateThis(Time time)
        {
            if (Player.TryGetUuid(this.Run.Runner, out Uuid id))
                this.face.SetPlayer(id);
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.face = this.First<UIAvatar>();
            this.face.Scale = 4;
            this.face.InitializeRecursive(screen);
            this.face.SetPlayer(this.Run.Runner);
            this.face.Glow();

            this.First<UITextBlock>("name").SetText(this.Run.Runner);
            this.First<UITextBlock>("igt").SetText(this.Run.InGameTime.ToString("hh':'mm':'ss"));
            this.First<UITextBlock>("status").SetText(this.Run.Status);

            int days = (int)(DateTime.UtcNow - this.Run.Date).TotalDays;
            if (days is 0)
                this.First<UITextBlock>("date").SetText("Set Today");
            else if (days is 1)
                this.First<UITextBlock>("date").SetText($"Set Yesterday");
            else
                this.First<UITextBlock>("date").SetText($"{days} days ago");
            
            base.InitializeRecursive(screen);
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;
            canvas.DrawRectangle(this.Bounds, this.BackColor, this.BorderColor, 2);
        }
    }
}

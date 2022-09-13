using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UICarouselCover : UIControl
    {
        private UIControl target;
        private Objective objective;
        private bool isComplete;
        private float targetHeight;
        private float height;
        private float speed;
        private float waitTime;
        private float waitRemaining;

        private int padding = 8;
        private int bottomPadding = 35;

        private Rectangle fill;

        public override void InitializeThis(UIScreen screen)
        {
            if (this.Parent is UIObjectiveFrame frame)
            {
                this.target = this.Parent.First("frame");
                this.objective = frame.Objective;
            }
            else if (this.Parent is UICriterion criterion)
            {
                this.target = this.Parent.First<UIPicture>();
                criterion.First("clarifying_icon")?.Collapse();
                this.objective = criterion.Objective;
            }

            
            if (this.target is null)
                this.target = this.Parent;

            this.speed = 5f;
            //this.bottomPadding = this.objective is Criterion ? 0 : this.bottomPadding;
            this.bottomPadding = 0;
            this.waitTime = this.objective is Criterion ? 0.5f : 0.5f;
            this.waitRemaining = this.waitTime;
        }

        protected override void UpdateThis(Time time)
        {
            this.isComplete = this.objective.IsComplete();
            if (this.isComplete)
            {
                this.targetHeight = this.target.Height + this.bottomPadding;
            }
            else
            {
                this.waitRemaining = this.waitTime;
                this.height = 0;
                this.targetHeight = 0;
                this.fill = default;
            }

            if (this.waitRemaining > 0)
            {
                this.waitRemaining -= (float)time.Delta;
                return;
            }
 
            this.height = MathHelper.Lerp(this.height, this.targetHeight, (float)(this.speed * time.Delta));
            int remaining = (int)this.targetHeight - (int)this.height;
            if (remaining > 0)
            {
                this.fill = new Rectangle(
                    this.target.Left - this.padding,
                    this.target.Top - this.padding + remaining,
                    this.target.Width + (this.padding),
                    this.target.Height + this.bottomPadding + (this.padding * 2) - remaining);
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.fill.Height > 0 && this.isComplete)
            {
                canvas.DrawRectangle(this.fill, Config.Overlay.GreenScreen, null, 0, Layer.Fore);
            }
        }
    }
}

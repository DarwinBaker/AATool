using System.Linq;
using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UICriteriaCarousel : UICarousel
    {
        public override void InitializeThis(UIScreen screen)
        {
            this.RefreshSourceList();
        }

        protected override void UpdateThis(Time time)
        {
            this.Fill();

            if (Tracker.ProgressChanged || Tracker.DesignationsChanged)
            {
                this.RefreshSourceList();

                //cover overlay items that have since been completed
                for (int i = this.Children.Count - 1; i >= 0; i--)
                {
                    if ((this.Children[i] as UICriterion).HideFromOverlay)
                    {
                        var cover = new UICarouselCover();
                        this.Children[i].AddControl(cover);
                        cover.InitializeThis(this.Root());
                    }
                } 
            }

            base.UpdateThis(time);

            this.Style();
        }

        protected override void RefreshSourceList()
        {
            //populate source list with all criteria
            this.SourceList.Clear();
            this.SourceList.AddRange(Tracker.RemainingCriteria.Values);

        }

        protected override void Fill()
        {
            //calculate widths
            int x;
            if (this.Children.Count > 0)
                x = this.RightToLeft ? this.Children.Last().Right : this.Width - this.Children.Last().Left;
            else
                x = this.Children.Count > 0 ? this.Children.Last().Right : 0;

            //while more controls will fit, add them
            while (x < this.Width)
            {
                if (this.SourceList.Count == 0)
                    return;

                if (this.NextIndex >= this.SourceList.Count)
                    this.NextIndex = 0;

                UIControl control = this.NextControl();
                
                control.InitializeRecursive(this.Root());
                control.ResizeRecursive(this.Bounds);

                if (this.RightToLeft)
                    control.MoveTo(new Point(x, this.Inner.Top));
                else
                    control.MoveTo(new Point(this.Width - x - control.Width, this.Inner.Top));
                this.AddControl(control);

                this.NextIndex++;
                x += this.Children[0].Width;
            }
        }

        protected override UIControl NextControl()
        {
            var criterion = this.SourceList[this.NextIndex] as Criterion;
            var control = new UICriterion(3) {
                IsStatic = true,
            };
            control.SetObjective(criterion);
            //fix ambiguity between some criteria of different advancements
            if (Config.Overlay.ClarifyAmbiguous && criterion.Icon is "hoglin" or "cat" or "tuxedo")
            {
                var advIcon = new UIPicture() {
                    Name = "clarifying_icon",
                    FlexWidth = new Size(48),
                    FlexHeight = new Size(48),
                    HorizontalAlign = HorizontalAlign.Left,
                    VerticalAlign = VerticalAlign.Top,
                    Margin = new Margin(-18, 0, -9, 0),
                };
                advIcon.SetTexture(criterion.Owner.Icon);
                advIcon.ResizeThis(control.Inner);
                control.AddControl(advIcon);
            }
            return control;
        }

        private void Style()
        {
            if (Config.Overlay.ClarifyAmbiguous.Changed)
            {
                foreach (UIControl criterion in this.Children)
                    criterion.First("clarifying_icon")?.SetVisibility(Config.Overlay.ClarifyAmbiguous);
            }
        }
    }
}
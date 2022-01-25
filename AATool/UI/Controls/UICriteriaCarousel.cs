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
            this.UpdateSourceList();
        }

        protected override void UpdateThis(Time time)
        {
            this.UpdateSourceList();
            this.Fill();

            //remove ones that have been completed
            for (int i = this.Children.Count - 1; i >= 0; i--)
            {
                if ((this.Children[i] as UICriterion).IsCompleted)
                    this.Children.RemoveAt(i);
            }

            base.UpdateThis(time);
        }

        protected override void UpdateSourceList()
        {
            //populate source list with all criteria
            this.SourceList.Clear();
            foreach (Criterion criterion in Tracker.Criteria.Values)
            {
                if (!criterion.CompletedByAnyone())
                    this.SourceList.Add(criterion);
            }

            //remove all completed criteria from pool if configured to do so
            for (int i = this.SourceList.Count - 1; i >= 0; i--)
            {
                if ((this.SourceList[i] as Criterion).CompletedByAnyone())
                    this.SourceList.RemoveAt(i);
            }
        }

        protected override void Fill()
        {
            //calculate widths
            int x = this.Children.Count > 0 ? this.Children.Last().Right : 0;
            if (this.Children.Count > 0)
                x = this.RightToLeft ? this.Children.Last().Right : this.Width - this.Children.Last().Left;

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
                AdvancementID = criterion.Owner.Id,
                CriterionID = criterion.Id
            };

            //fix ambiguity between some criteria of different advancements
            if (Config.Overlay.ClarifyAmbiguous && criterion.Icon is "hoglin" or "cat" or "tuxedo")
            {
                var advIcon = new UIPicture() {
                    FlexWidth = new Size(48),
                    FlexHeight = new Size(48),
                    HorizontalAlign = HorizontalAlign.Left,
                    VerticalAlign = VerticalAlign.Top,
                    Margin = new Margin(-16, 0, -16, 0),
                };
                advIcon.SetTexture(criterion.Owner.Icon);
                advIcon.ResizeThis(control.Inner);
                control.AddControl(advIcon);
            }
            return control;
        }
    }
}
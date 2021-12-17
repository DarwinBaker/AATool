using AATool.Data;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AATool.UI.Controls
{
    class UICriteriaCarousel : UICarousel
    {
        public override void InitializeRecursive(UIScreen screen)
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
            foreach (Criterion criterion in Tracker.AllCriteria.Values)
            {
                if (!criterion.CompletedByAnyone())
                    this.SourceList.Add(criterion);
            }

            //remove all completed criteria from pool if configured to do so
            for (int i = SourceList.Count - 1; i >= 0; i--)
                if ((SourceList[i] as Criterion).CompletedByAnyone())
                    SourceList.RemoveAt(i);
        }

        protected override void Fill()
        {
            //calculate widths
            int x = this.Children.Count > 0 ? this.Children.Last().Right : 0;
            if (this.Children.Count > 0)
                x = this.RightToLeft ? this.Children.Last().Right : this.Width - this.Children.Last().Left;

            //while more controls will fit, add them
            int attempts = 0;
            while (x < this.Width)
            {
                if (SourceList.Count == 0)
                    return;

                if (NextIndex >= SourceList.Count)
                    NextIndex = 0;

                var control = NextControl();
                
                control.InitializeRecursive(Root());
                control.ResizeRecursive(Bounds);

                if (RightToLeft)
                    control.MoveTo(new Point(x, Content.Top));
                else
                    control.MoveTo(new Point(Width - x - control.Width, Content.Top));
                AddControl(control);

                NextIndex++;
                x += Children[0].Width;
            }
        }

        protected override UIControl NextControl()
        {
            var criterion = this.SourceList[this.NextIndex] as Criterion;
            var control = new UICriterion(3) {
                IsStatic = true,
                AdvancementID = criterion.ParentAdvancement.Id,
                CriterionID = criterion.ID
            };

            //fix ambiguity between some criteria of different advancements
            if (criterion.Icon is "hoglin" or "cat" or "tuxedo")
            {
                var advIcon = new UIPicture() {
                    FlexWidth = new Size(48),
                    FlexHeight = new Size(48),
                    HorizontalAlign = HorizontalAlign.Left,
                    VerticalAlign = VerticalAlign.Top,
                    Margin = new Margin(-16, 0, -16, 0),
                };
                advIcon.SetTexture(criterion.ParentAdvancement.Icon);
                advIcon.ResizeThis(control.Content);
                control.AddControl(advIcon);
            }
            return control;
        }
    }
}
using AATool.DataStructures;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AATool.UI.Controls
{
    class UICriteriaCarousel : UICarousel
    {
        public override void InitializeRecursive(Screen screen)
        {
            UpdateSourceList();
        }

        protected override void UpdateThis(Time time)
        {
            UpdateSourceList();

            Fill();
            for (int i = Children.Count - 1; i >= 0; i--)
                if ((Children[i] as UICriterion).IsCompleted)
                    Children.RemoveAt(i);

            base.UpdateThis(time);
        }

        protected override void UpdateSourceList()
        {
            SourceList = new List<object>();
            foreach (Advancement advancement in GetRootScreen().AdvancementTracker.AdvancementList.Values)
                if (advancement.HasCriteria)
                    foreach (Criterion criterion in advancement.Criteria.Values)
                        if (!criterion.IsCompleted)
                            SourceList.Add(criterion);
        }

        protected override void Fill()
        {
            //calculate widths
            int totalWidth = Children.Count > 0 ? Children.Last().Right : 0;

            //while more controls will fit, add them
            while (totalWidth < Width)
            {
                if (SourceList.Count == 0)
                    return;

                if (NextIndex >= SourceList.Count)
                    NextIndex = 0;

                var control = NextControl();
                AddControl(control);

                control.InitializeRecursive(GetRootScreen());
                control.ResizeRecursive(Rectangle);
                control.MoveTo(new Point(totalWidth, ContentRectangle.Top));
                NextIndex++;
                totalWidth += Children[0].Width;
            }
        }

        protected override UIControl NextControl()
        {
            var criterion = SourceList[NextIndex] as Criterion;
            var control = new UICriterion(3);
            control.IsStatic = true;
            control.AdvancementName = criterion.AdvancementID;
            control.CriterionName = criterion.ID;
            return control;
        }
    }
}
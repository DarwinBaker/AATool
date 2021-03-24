using AATool.DataStructures;
using AATool.Settings;
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

            //remove ones that have been completed
            for (int i = Children.Count - 1; i >= 0; i--)
                if ((Children[i] as UICriterion).IsCompleted)
                    Children.RemoveAt(i);

            base.UpdateThis(time);
        }

        protected override void UpdateSourceList()
        {
            //populate source list with all criteria
            SourceList = new List<object>();
            if (TrackerSettings.IsPostExplorationUpdate)
            {
                foreach (Criterion criterion in GetRootScreen().AdvancementTracker.FullCriteriaList.Values)
                    if (!criterion.IsCompleted)
                        SourceList.Add(criterion);
            }
            else
            {
                foreach (Criterion criterion in GetRootScreen().AchievementTracker.FullCriteriaList.Values)
                    if (!criterion.IsCompleted)
                        SourceList.Add(criterion);
            }
                

            //remove all completed criteria from pool if configured to do so
            for (int i = SourceList.Count - 1; i >= 0; i--)
                if ((SourceList[i] as Criterion).IsCompleted)
                    SourceList.RemoveAt(i);

            //remove all criteria not marked as favorites from pool if configured to do so
            if (OverlaySettings.Instance.OnlyShowFavorites)
                for (int i = SourceList.Count - 1; i >= 0; i--)
                {
                    var criterion = SourceList[i] as Criterion;
                    if (!OverlaySettings.Instance.Favorites.Criteria.Contains(criterion.ParentID + "/" + criterion.ID))
                        SourceList.RemoveAt(i);
                }
        }

        protected override void Fill()
        {
            //calculate widths
            int x = Children.Count > 0 ? Children.Last().Right : 0;
            if (Children.Count > 0)
                x = OverlaySettings.Instance.RightToLeft ? Children.Last().Right : Width - Children.Last().Left;

            //while more controls will fit, add them
            while (x < Width)
            {
                if (SourceList.Count == 0)
                    return;

                if (NextIndex >= SourceList.Count)
                    NextIndex = 0;

                var control = NextControl();
                

                control.InitializeRecursive(GetRootScreen());
                control.ResizeRecursive(Rectangle);

                if (OverlaySettings.Instance.RightToLeft)
                    control.MoveTo(new Point(x, ContentRectangle.Top));
                else
                    control.MoveTo(new Point(Width - x - control.Width, ContentRectangle.Top));
                AddControl(control);

                NextIndex++;
                x += Children[0].Width;
            }
        }

        protected override UIControl NextControl()
        {
            var criterion = SourceList[NextIndex] as Criterion;
            var control = new UICriterion(3);
            control.IsStatic = true;
            control.AdvancementName = criterion.ParentID;
            control.CriterionName = criterion.ID;
            return control;
        }
    }
}
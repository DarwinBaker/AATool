using AATool.DataStructures;
using AATool.Settings;
using AATool.UI.Screens;
using System.Collections.Generic;
using System.Linq;

namespace AATool.UI.Controls
{
    class UIAdvancementCarousel : UICarousel
    {
        private bool showLabels;
        private bool onlyRemaining;
        private bool onlyFavorites;
         
        public override void InitializeRecursive(Screen screen)
        {
            UpdateSourceList();
        }

        protected override void UpdateThis(Time time)
        {
            UpdateSourceList();

            //update show remaining only
            if (onlyRemaining != OverlaySettings.Instance.HideCompleted)
            {
                onlyRemaining = OverlaySettings.Instance.HideCompleted;
                Clear();
            }

            //update show important only
            if (onlyFavorites != OverlaySettings.Instance.OnlyShowFavorites)
            {
                onlyFavorites = OverlaySettings.Instance.OnlyShowFavorites;
                Clear();
            }

            //update text visibility
            if (showLabels != OverlaySettings.Instance.ShowLabels)
            {
                showLabels = OverlaySettings.Instance.ShowLabels;
                if (OverlaySettings.Instance.ShowLabels)
                    foreach (var control in Children)
                        (control as UIAdvancement).ShowText();
                else
                    foreach (var control in Children)
                        (control as UIAdvancement).HideText();
            }

            Fill();

            //remove completed advancements from carousel if configured to do so
            if (onlyRemaining)
                for (int i = Children.Count - 1; i >= 0; i--)
                    if ((Children[i] as UIAdvancement).IsCompleted)
                        Children.RemoveAt(i);

            base.UpdateThis(time);
        }

        protected override void UpdateSourceList()
        {
            var advancements = GetRootScreen().AdvancementTracker.AdvancementList.Values.ToList();
            SourceList = new List<object>(advancements);

            //remove all completed advancements from pool if configured to do so
            if (OverlaySettings.Instance.HideCompleted)
                for (int i = SourceList.Count - 1; i >= 0; i--)
                    if (advancements[i].IsCompleted)
                        SourceList.RemoveAt(i);

            //remove all advancements not marked as favorites from pool if configured to do so
            if (OverlaySettings.Instance.OnlyShowFavorites)
                for (int i = SourceList.Count - 1; i >= 0; i--)
                    if (!OverlaySettings.Instance.Favorites.Contains((SourceList[i] as Advancement).ID))
                        SourceList.RemoveAt(i);
        }

        protected override UIControl NextControl()
        {
            //instantiate next control
            var control = new UIAdvancement(3);
            control.AdvancementName = (SourceList[NextIndex] as Advancement).ID;
            control.InitializeRecursive(GetRootScreen());
            if (!showLabels) 
                control.HideText();
            return control;
        }
    }
}

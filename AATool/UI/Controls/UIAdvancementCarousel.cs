using AATool.Data;
using AATool.Settings;
using System.Collections.Generic;

namespace AATool.UI.Controls
{
    class UIAdvancementCarousel : UICarousel
    {  
        protected override void UpdateThis(Time time)
        {
            this.UpdateSourceList();

            if (Config.Tracker.GameVersionChanged())
                this.Clear();

            //update text visibility
            if (Config.Overlay.ShowLabelsChanged())
            {
                if (Config.Overlay.ShowLabels)
                {
                    foreach (var control in Children)
                        (control as UIAdvancement).ShowText();
                }
                else
                {
                    foreach (var control in Children)
                        (control as UIAdvancement).HideText();
                }
            }

            Fill();

            //remove completed advancements from carousel
            for (int i = this.Children.Count - 1; i >= 0; i--)
            {
                if ((this.Children[i] as UIAdvancement).IsCompleted)
                    this.Children.RemoveAt(i);
            }

            base.UpdateThis(time);
        }

        protected override void UpdateSourceList()
        {
            var advancements = new List<Advancement>();
            foreach (Advancement advancement in Tracker.AllAdvancements.Values)
                advancements.Add(advancement);

            this.SourceList = new List<object>(advancements);

            //remove all completed advancements from pool
            for (int i = this.SourceList.Count - 1; i >= 0; i--)
            {
                if (advancements[i].CompletedByAnyone())
                    this.SourceList.RemoveAt(i);
            }
        }

        protected override UIControl NextControl()
        {
            //instantiate next control
            var nextAdvancement = this.SourceList[this.NextIndex] as Advancement;
            var control = new UIAdvancement(3) {
                AdvancementName = nextAdvancement.Id
            };
            control.InitializeRecursive(this.GetRootScreen());
            if (!Config.Overlay.ShowLabels) 
                control.HideText();
            return control;
        }
    }
}

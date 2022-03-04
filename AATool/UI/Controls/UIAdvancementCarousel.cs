using AATool.Configuration;
using AATool.Data;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using System.Collections.Generic;

namespace AATool.UI.Controls
{
    class UIAdvancementCarousel : UICarousel
    {  
        protected override void UpdateThis(Time time)
        {
            this.UpdateSourceList();

            if (Tracker.ObjectivesChanged)
                this.Clear();

            //update text visibility
            if (Config.Overlay.ShowLabels.Changed)
            {
                if (Config.Overlay.ShowLabels)
                {
                    foreach (UIControl control in this.Children)
                        (control as UIObjectiveFrame).ShowText();
                }
                else
                {
                    foreach (UIControl control in this.Children)
                        (control as UIObjectiveFrame).HideText();
                }
            }

            this.Fill();

            //remove completed advancements from carousel
            for (int i = this.Children.Count - 1; i >= 0; i--)
            {
                if ((this.Children[i] as UIObjectiveFrame).ObjectiveCompleted)
                    this.Children.RemoveAt(i);
            }

            base.UpdateThis(time);
        }

        protected override void UpdateSourceList()
        {
            var objectives = new List<Objective>(Tracker.Category.GetOverlayObjectives());
            this.SourceList = new List<object>(objectives);

            //remove all completed advancements from pool
            for (int i = this.SourceList.Count - 1; i >= 0; i--)
            {
                if (objectives[i].CompletedByAnyone())
                    this.SourceList.RemoveAt(i);
            }

            //remove unimportant advancements if half percent
            if (Tracker.Category is HalfPercent)
            {
                for (int i = this.SourceList.Count - 1; i >= 0; i--)
                {
                    if (!(this.SourceList[i] as Advancement).UsedInHalfPercent)
                        this.SourceList.RemoveAt(i);
                }
            }
        }

        protected override UIControl NextControl()
        {
            //instantiate next control
            var next = this.SourceList[this.NextIndex] as Objective;
            var control = new UIObjectiveFrame(next, 3);
            this.Style(control);
            return control;
        }

        private void Style(UIObjectiveFrame frame)
        {
            if (frame.Objective is Death)
            {
                frame.FlexWidth = new Size(42);
                if (frame.Objective?.Id is "death.fall.accident.water")
                {
                    var slab = new UIPicture("inverted_stone_slab") {
                        FlexWidth = new (48),
                        FlexHeight = new (48),
                        VerticalAlign = VerticalAlign.Top,
                        Margin = new Margin(0, 0, 15, 0),
                        Layer = Layer.Fore,
                    };
                    frame.AddControl(slab);
                }
            }
                

            frame.InitializeRecursive(this.Root());
            if (!Config.Overlay.ShowLabels)
                frame.HideText();
        }
    }
}

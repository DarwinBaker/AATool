using AATool.Configuration;
using AATool.Data;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using System.Collections.Generic;

namespace AATool.UI.Controls
{
    class UIObjectiveCarousel : UICarousel
    {  
        protected override void UpdateThis(Time time)
        {
            if (Tracker.Invalidated || Config.Overlay.Enabled.Changed)
                this.RefreshSourceList();

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

            if (Tracker.ProgressChanged)
            {
                //remove existing overlay advancements that have since been completed
                for (int i = this.Children.Count - 1; i >= 0; i--)
                {
                    if ((this.Children[i] as UIObjectiveFrame).Objective?.IsComplete() is true)
                        this.Children.RemoveAt(i);
                }
            }
            base.UpdateThis(time);
        }

        protected override void RefreshSourceList()
        {
            this.SourceList.Clear();
            this.SourceList.AddRange(Tracker.Category.GetOverlayObjectives());

            //remove completed advancements from pool
            for (int i = this.SourceList.Count - 1; i >= 0; i--)
            {
                if ((this.SourceList[i] as Objective).IsComplete())
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

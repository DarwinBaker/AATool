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
            var objectives = new List<Objective>();
            if (Tracker.Category is MonstersHunted monstersHunted)
            {
                foreach (Objective monster in monstersHunted.AllCriteria)
                    objectives.Add(monster);
            }
            else if (Tracker.Category is BalancedDiet balancedDiet)
            {
                foreach (Objective monster in balancedDiet.AllCriteria)
                    objectives.Add(monster);
            }
            else if (Tracker.Category is AdventuringTime adventuringTime)
            {
                foreach (Objective monster in adventuringTime.AllCriteria)
                    objectives.Add(monster);
            }
            else if (Tracker.Category is AllAchievements)
            {
                foreach (Objective advancement in Tracker.Achievements.All.Values)
                    objectives.Add(advancement);
            }
            else
            {
                foreach (Objective advancement in Tracker.Advancements.All.Values)
                    objectives.Add(advancement);
            }
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
            control.InitializeRecursive(this.Root());
            if (!Config.Overlay.ShowLabels) 
                control.HideText();
            return control;
        }
    }
}

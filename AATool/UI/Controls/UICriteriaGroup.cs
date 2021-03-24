using AATool.DataStructures;
using AATool.Settings;
using AATool.UI.Screens;
using System.Xml;

namespace AATool.UI.Controls
{
    class UICriteriaGroup : UIPanel
    {
        public string AdvancementName;

        private Advancement advancement;
        private string goalName;
        private UITextBlock label;
        private UIProgressBar bar;

        public UICriteriaGroup()
        {
            InitializeFromSourceDocument();
        }

        public override void InitializeRecursive(Screen screen)
        {
            if (TrackerSettings.IsPostExplorationUpdate)
                advancement = screen.AdvancementTracker.Advancement(AdvancementName);
            else
                advancement = screen.AchievementTracker.Achievement(AdvancementName);
            if (advancement == null)
                return;

            goalName = advancement.CriteriaGoal;

            var adv = GetFirstOfType(typeof(UIAdvancement), true) as UIAdvancement;
            if (adv != null)
                adv.AdvancementName = AdvancementName;

            var flow = GetFirstOfType(typeof(UIFlowPanel), true) as UIFlowPanel;
            if (flow != null)
            {
                foreach (var criterion in advancement.Criteria)
                {
                    var crit = new UICriterion();
                    crit.AdvancementName = AdvancementName;
                    crit.CriterionName = criterion.Key;
                    flow.AddControl(crit);
                }
            }

            label = GetControlByName("progress", true) as UITextBlock;
            bar = GetControlByName("bar", true) as UIProgressBar;
            bar?.SetMax(advancement.Criteria.Count);

            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            //update progress display
            bar?.SetValue(advancement.CriteriaCompleted);

            if (label == null) return;
            label.SetText(goalName);
            label.Append(": ");
            label.Append(advancement.CriteriaCompleted.ToString());
            label.Append(" / ");
            label.Append(advancement.CriteriaCount.ToString());
            label.Append(" (");
            label.Append(advancement.CriteriaPercent.ToString());
            label.Append("%)");
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            AdvancementName = ParseAttribute(node, "advancement", string.Empty);
        }
    }
}

using System.Collections.Generic;
using AATool.Data.Speedrunning;
using AATool.UI.Screens;

namespace AATool.UI.Controls
{
    internal class UIRecordsAllBlocks : UIPanel
    {
        public UIRecordsAllBlocks() : base()
        {
            this.BuildFromTemplate();
        }

        public override void InitializeThis(UIScreen screen)
        {
            var children = new List<UIControl>();
            this.GetTreeRecursive(children);

            foreach (UIControl control in children)
            {
                if (control is UIButton button)
                {
                    button.OnClick += this.OnClick;
                }
            }
        }

        private void OnClick(UIControl sender)
        {
            if (sender?.Tag is string runnerName)
            {
                if (UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
                {
                    RunnerProfile.SetCurrentName(runnerName);
                    UIMainScreen.SetActiveTab(UIMainScreen.RunnerProfileTab);
                }
            }
        }
    }
}

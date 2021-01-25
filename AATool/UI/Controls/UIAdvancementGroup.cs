using AATool.DataStructures;
using AATool.UI.Screens;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIAdvancementGroup : UIFlowPanel
    {
        public string GroupName;

        private AdvancementGroup group;

        public UIAdvancementGroup()
        {
            InitializeFromSourceDocument();
        }

        public override void InitializeRecursive(Screen screen)
        {
            group = screen.AdvancementTracker.Group(GroupName);
            if (group != null)
            {
                foreach (var advancement in group.Advancements.Keys)
                {
                    var temp = new UIAdvancement();
                    temp.AdvancementName = advancement;
                    AddControl(temp);
                }
            }
            base.InitializeRecursive(screen);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            GroupName = ParseAttribute(node, "group", "");
        }
    }
}

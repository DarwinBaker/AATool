using AATool.Data;
using AATool.Settings;
using AATool.UI.Screens;
using System.Collections.Generic;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIAdvancementGroup : UIFlowPanel
    {
        public string GroupName;

        private AdvancementGroup group;

        public UIAdvancementGroup()
        {
            this.BuildFromSourceDocument();
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            if (Config.Main.CompactMode)
            {
                this.CellWidth  = 66;
                this.CellHeight = 68;
                this.DrawMode = DrawMode.ChildrenOnly;
            }

            Tracker.TryGetAdvancementGroup(this.GroupName, out group);
            if (this.group != null)
            {
                foreach (KeyValuePair<string, Advancement> advancement in this.group.Advancements)
                {
                    //skip hidden advancements
                    if (!Config.Main.CompactMode && advancement.Value.HiddenWhenRelaxed)
                        continue;
                    else if (Config.Main.CompactMode && advancement.Value.HiddenWhenCompact)
                        continue;

                    var temp = new UIAdvancement();
                    temp.AdvancementName = advancement.Key;
                    this.AddControl(temp);
                }
            }
            base.InitializeRecursive(screen);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.GroupName = ParseAttribute(node, "group", string.Empty);
        }
    }
}

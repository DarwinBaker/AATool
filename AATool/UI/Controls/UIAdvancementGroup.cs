using System.Collections.Generic;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.UI.Screens;

namespace AATool.UI.Controls
{
    class UIAdvancementGroup : UIFlowPanel
    {
        public string GroupId { get; private set; }
        public string StartId { get; private set; }
        public string EndId { get; private set; }

        private readonly HashSet<string> excluded = new ();

        public UIAdvancementGroup()
        {
            this.BuildFromTemplate();
        }

        public override void InitializeThis(UIScreen screen)
        {
            if (Config.Main.CompactMode)
            {
                this.CellWidth  = 66;
                this.CellHeight = 68;
                this.DrawMode = DrawMode.ChildrenOnly;
            }
            this.Populate();
        }

        private void Populate()
        {
            //start adding from beginning if no start id specified 
            bool inBounds = string.IsNullOrEmpty(this.StartId);
            if (Tracker.TryGetAdvancementGroup(this.GroupId, out HashSet<Advancement> group))
            {
                foreach (Advancement advancement in group)
                {
                    //check if advancement should be added
                    inBounds |= advancement.Id == this.StartId;
                    if (inBounds && !this.excluded.Contains(advancement.Id))
                        this.AddControl(new UIObjectiveFrame(advancement));

                    //skip rest of advancements if end of specified range reached
                    if (advancement.Id == this.EndId)
                        break;
                }
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.GroupId = Attribute(node, "group", string.Empty);
            this.StartId = Attribute(node, "start", string.Empty);
            this.EndId = Attribute(node, "end", string.Empty);

            //parse comma separated string of advancements to exclude
            string skipList = Attribute(node, "exclude", string.Empty);
            string[] ids = skipList.Split(',');
            bool hasHiddenAdvancements = ids.Length > 1 
                || !(ids.Length is 1 && string.IsNullOrEmpty(ids[0]));
            if (hasHiddenAdvancements)
                this.excluded.UnionWith(ids);
        }
    }
}

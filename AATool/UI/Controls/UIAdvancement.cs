using AATool.DataStructures;
using AATool.UI.Screens;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIAdvancement : UIControl
    {
        public string AdvancementName;

        private Advancement advancement;
        private UIPicture frame;
        private UITextBlock label;
        private int scale;

        public bool IsCompleted => advancement?.IsCompleted ?? false;

        public UIAdvancement() 
        {
            InitializeFromSourceDocument();
            scale = 2;
        }

        public UIAdvancement(int scale = 2) : this()
        {
            this.scale = scale;
        }

        public void ShowText() => label.Expand();
        public void HideText() => label.Collapse();

        public override void InitializeRecursive(Screen screen)
        {
            advancement = screen.AdvancementTracker.Advancement(AdvancementName);

            int textScale = scale < 3 ? 1 : 2;
            FlexWidth  *= Math.Min(scale + textScale - 1, 4);
            FlexHeight *= scale;
            Padding = new Margin(0, 0, 4 * scale, 0);

            frame = GetControlByName("frame", true) as UIPicture;
            if (frame != null)
            {
                frame.FlexWidth  *= scale;
                frame.FlexHeight *= scale;
            }
            
            var icon = GetControlByName("icon", true) as UIPicture;
            if (icon != null) 
            {
                icon.FlexWidth  *= scale;
                icon.FlexHeight *= scale;
                icon.SetTexture(advancement.Icon);
            }

            label = GetControlByName("label", true) as UITextBlock;
            if (label != null)
            {
                label.Margin = new Margin(0, 0, (int)frame.FlexHeight.InternalValue, 0);
                label.SetFont("minecraft", 12 * textScale);
                label.SetText(advancement.Name);
            }         
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            frame?.SetTexture(advancement.CurrentFrame);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            AdvancementName = ParseAttribute(node, "id", string.Empty);
            scale = ParseAttribute(node, "scale", scale);
        }
    }
}

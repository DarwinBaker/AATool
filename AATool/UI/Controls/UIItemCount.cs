using AATool.DataStructures;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIItemCount : UIControl
    {
        public string ItemName;

        private ItemStats itemCount;
        private UIPicture frame;
        private UIPicture icon;
        private UITextBlock label;
        private int scale;

        public UIItemCount() 
        {
            InitializeFromSourceDocument();
            scale = 2;
        }

        public UIItemCount(int scale = 2) : this()
        {
            this.scale = scale;
        }

        public override void InitializeRecursive(Screen screen)
        {
            itemCount = screen.StatisticsTracker.ItemCount(ItemName);
            if (itemCount == null)
                return;

            if (itemCount.IsEstimate)
                DrawMode = DrawMode.ThisOnly;

            int textScale = scale < 3 ? 1 : 2;
            FlexWidth *= Math.Min(scale + textScale - 1, 4);
            FlexHeight *= scale;
            Padding = new Margin(0, 0, 4 * scale, 0);

            frame = GetControlByName("frame", true) as UIPicture;
            if (frame != null)
            {
                frame.FlexWidth *= scale;
                frame.FlexHeight *= scale;
            }
            
            icon = GetControlByName("icon", true) as UIPicture;
            if (icon != null)
            {
                icon.FlexWidth *= scale;
                icon.FlexHeight *= scale;
                icon.SetTexture(itemCount.Icon);
            }

            label = GetControlByName("label", true) as UITextBlock;
            if (label != null)
            {
                label.Margin = new Margin(0, 0, (int)frame.FlexHeight.InternalValue, 0);
                label.SetFont("minecraft", 12 * textScale);
            }

            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            //update count display
            if (itemCount != null)
            {
                frame?.SetTexture(itemCount.CurrentFrame);

                int percent = (int)Math.Round((float)itemCount.PickedUp / itemCount.TargetCount * 100);
                if (itemCount.TargetCount == 1)
                    label?.SetText(itemCount.Name);
                else if (!itemCount.IsEstimate)
                    label?.SetText(itemCount.Name + "\n" + itemCount.PickedUp + " / " + itemCount.TargetCount);
                else if (percent == 0)
                    label?.SetText(itemCount.Name + "\n" + Math.Min(percent, 100) + "%");
                else
                    label?.SetText(itemCount.Name + "\n~" + Math.Min(percent, 100) + "%");
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            ItemName    = ParseAttribute(node, "id", "");
            scale = ParseAttribute(node, "scale", scale);
        }

        public override void DrawThis(Display display)
        {
            if (itemCount?.IsEstimate ?? false)
            {
                float opacity = (float)itemCount.PickedUp / itemCount.TargetCount;
                if (frame != null)
                {
                    frame.DrawThis(display);
                    display.Draw(ItemStats.FrameComplete, frame.ContentRectangle, Color.White * opacity);
                }
                if (icon != null)
                    display.Draw(itemCount.Icon, icon.ContentRectangle, Color.White);
                label?.DrawThis(display);
            }
        }
    }
}

using AATool.DataStructures;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIItemCount : UIControl
    {
        public string ItemName;

        private Statistic itemCount;
        private UIPicture frame;
        private UIPicture icon;
        private UITextBlock label;
        private int scale;
        private float glowRotation;

        public bool IsCompleted => itemCount?.IsCompleted ?? false;

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
            //FlexWidth *= Math.Min(scale + textScale - 1, 4);
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
                icon.SetLayer(Layer.Fore);
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
            itemCount = GetRootScreen().StatisticsTracker.ItemCount(ItemName);

            //update count display
            if (itemCount != null)
            {
                frame?.SetTexture(itemCount.CurrentFrame);
                icon?.SetTexture(itemCount.Icon);

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

            if (IsCompleted)
                glowRotation += (float)time.Delta * 0.25f;
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            ItemName = ParseAttribute(node, "id", string.Empty);
            scale    = ParseAttribute(node, "scale", scale);
        }

        public override void DrawRecursive(Display display)
        {
            if (IsCollapsed)
                return;

            frame?.DrawRecursive(display);
            if (itemCount?.IsEstimate ?? false)
            {
                float opacity = (float)itemCount.PickedUp / itemCount.TargetCount;
                if (frame != null)
                {
                    frame.DrawThis(display);
                    display.Draw(Statistic.FRAME_COMPLETE, frame.ContentRectangle, Color.White * opacity);
                }

                if (IsCompleted && MainSettings.Instance.RenderCompletionGlow)
                    display.Draw("frame_glow", frame.Center.ToVector2(), glowRotation, Color.White * opacity, Layer.Glow);
            }
            else
                if (IsCompleted && MainSettings.Instance.RenderCompletionGlow)
                    display.Draw("frame_glow", frame.Center.ToVector2(), glowRotation, Color.White, Layer.Glow);

            icon?.DrawRecursive(display);
            label?.DrawRecursive(display);
        }

        public override void DrawThis(Display display)
        {
            
        }
    }
}

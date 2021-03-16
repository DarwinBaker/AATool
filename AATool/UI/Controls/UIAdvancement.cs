using AATool.DataStructures;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIAdvancement : UIControl
    {
        public string AdvancementName;

        private Advancement advancement;
        private UIPicture frame;
        private UIPicture icon;
        private UITextBlock label;
        private int scale;
        private float glowRotation;

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

            icon = GetControlByName("icon", true) as UIPicture;
            if (icon != null) 
            {
                icon.FlexWidth  *= scale;
                icon.FlexHeight *= scale;
                icon.SetTexture(advancement.Icon);
                icon.SetLayer(Layer.Fore);
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
            if (IsCompleted)
                glowRotation += (float)time.Delta * 0.25f;
        }

        public override void DrawRecursive(Display display)
        {
            frame?.DrawRecursive(display);

            if (IsCompleted && MainSettings.Instance.RenderCompletionGlow && scale == 2)
                display.Draw("frame_glow", frame.Center.ToVector2(), glowRotation, Color.White, Layer.Glow);

            icon?.DrawRecursive(display);
            label?.DrawRecursive(display);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            AdvancementName = ParseAttribute(node, "id", string.Empty);
            scale = ParseAttribute(node, "scale", scale);
        }
    }
}

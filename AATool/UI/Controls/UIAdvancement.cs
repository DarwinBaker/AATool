using AATool.DataStructures;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIAdvancement : UIControl
    {
        public Advancement Advancement { get; private set; }
        public string AdvancementName;
        
        private UIPicture frame;
        private UIPicture icon;
        private UITextBlock label;
        private int scale;
        private float glowRotation;

        public bool IsCompleted => Advancement?.IsCompleted ?? false;
        public Point IconCenter => icon?.Center ?? Point.Zero;

        private List<UIAdvancement> childAchievements;

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
            if (TrackerSettings.IsPostExplorationUpdate)
                Advancement = screen.AdvancementTracker.Advancement(AdvancementName);
            else
                Advancement = screen.AchievementTracker.Achievement(AdvancementName);
            if (Advancement == null)
                return;

            Name = Advancement.ID;
            int textScale = scale < 3 ? 1 : 2;
            FlexWidth  *= Math.Min(scale + textScale - 1, 4);
            FlexHeight *= scale;

            if (TrackerSettings.IsPostExplorationUpdate)
                Padding = new Margin(0, 0, 4 * scale, 0);
            else
                Padding = new Margin(0, 0, 6, 0);

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
                icon.SetTexture(Advancement.Icon);
                icon.SetLayer(Layer.Fore);
            }

            label = GetControlByName("label", true) as UITextBlock;
            if (label != null)
            {
                label.Margin = new Margin(0, 0, (int)frame.FlexHeight.InternalValue, 0);
                label.SetFont("minecraft", 12 * textScale);
                label.SetText(Advancement.Name);

                if (TrackerSettings.IsPreExplorationUpdate && screen is MainScreen)
                    label.DrawBackground = true;
            }         
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            frame?.SetTexture(Advancement.CurrentFrame);
            if (IsCompleted)
                glowRotation += (float)time.Delta * 0.25f;
        }

        public override void DrawRecursive(Display display)
        {
            if (Advancement is Achievement && scale == 2)
            {
                if ((Advancement as Achievement).CanBeDoneYet)
                {
                    icon?.SetTint(Color.White);
                    frame?.SetTint(Color.White);
                    label?.SetTextColor(MainSettings.Instance.TextColor);
                }
                else
                {
                    icon?.SetTint(Color.Gray * 0.1f);
                    frame?.SetTint(Color.Gray * 0.2f);
                    label?.SetTextColor(MainSettings.Instance.TextColor * 0.5f);
                }
            }

            frame?.DrawRecursive(display);
            if (IsCompleted && MainSettings.Instance.RenderCompletionGlow)
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

using AATool.Data;
using AATool.Graphics;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIItemCount : UIControl
    {
        private const string FRAME_INCOMPLETE = "frame_count_incomplete";
        private const string FRAME_COMPLETE   = "frame_count_complete";

        public string ItemName;

        private Statistic itemCount;
        private UIPicture frame;
        private UIPicture icon;
        private UIGlowEffect glow;
        private UITextBlock label;
        private int scale;
        private bool isMainWindow;

        public bool IsCompleted => itemCount?.IsCompleted ?? false;

        public UIItemCount() 
        {
            this.BuildFromSourceDocument();
            this.scale = 2;
        }

        public UIItemCount(int scale = 2) : this()
        {
            this.scale = scale;
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.isMainWindow = screen is UIMainScreen;

            if (!Tracker.TryGetItem(this.ItemName, out this.itemCount))
                return;

            this.glow  = this.First<UIGlowEffect>();

            int textSize = 12 * ((Config.Overlay.Scale + 1) / 2);
            int textScale = this.scale < 3 ? 1 : 2;
            //FlexWidth *= Math.Min(scale + textScale - 1, 4);
            this.Padding = new Margin(0, 0, 4 * this.scale, 0);

            this.frame = this.First<UIPicture>("frame");
            if (this.frame != null)
            {
                this.frame.FlexWidth *= this.scale;
                this.frame.FlexHeight *= this.scale;
            }

            this.icon = this.First<UIPicture>("icon");
            if (this.icon != null)
            {
                this.icon.FlexWidth *= this.scale;
                this.icon.FlexHeight *= this.scale;
                this.icon.SetTexture(this.itemCount.Icon);
                this.icon.SetLayer(Layer.Fore);
            }

            this.label = this.First<UITextBlock>("label");
            if (this.label != null)
            {
                this.label.Margin = new Margin(0, 0, (int)this.frame.FlexHeight.InternalValue, 0);
                this.label.SetFont("minecraft", 12 * textScale);
            }

            //compact mode
            if (this.isMainWindow && Config.Main.CompactMode)
            {
                this.FlexWidth  = new Size(66);
                this.FlexHeight = new Size(68);
            }
            else
            {
                this.FlexHeight *= this.scale;
            }

            this.UpdateGlowBrightness(null);
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            Tracker.TryGetItem(this.ItemName, out this.itemCount);

            //update count display
            if (this.itemCount is not null)
            {
                this.UpdateGlowBrightness(time);

                this.frame?.SetTexture(this.itemCount.CurrentFrame);
                this.icon?.SetTexture(this.itemCount.Icon);

                int percent = (int)Math.Round((float)this.itemCount.PickedUp / this.itemCount.TargetCount * 100);

                if (Config.Main.CompactMode && this.scale is 2)
                { 
                    if (this.itemCount.TargetCount is 1)
                        this.label?.SetText(this.itemCount.PickedUp.ToString());
                    else if (!this.itemCount.IsEstimate)
                        this.label?.SetText(this.itemCount.PickedUp + " / " + this.itemCount.TargetCount);
                    else if (percent == 0)
                        this.label?.SetText(Math.Min(percent, 100) + "%");
                    else
                        this.label?.SetText("~" + Math.Min(percent, 100) + "%");
                }                   
                else if (this.itemCount.TargetCount is 1)
                    this.label?.SetText(this.itemCount.Name);
                else if (!this.itemCount.IsEstimate)
                    this.label?.SetText(this.itemCount.Name + "\n" + this.itemCount.PickedUp + "\0/\0" + this.itemCount.TargetCount);
                else if (percent == 0)
                    this.label?.SetText(this.itemCount.Name + "\n" + Math.Min(percent, 100) + "%");
                else
                    this.label?.SetText(this.itemCount.Name + "\n~" + Math.Min(percent, 100) + "%");
            }
        }

        private void UpdateGlowBrightness(Time time)
        {
            if (this.isMainWindow && Config.Main.CompletionGlow)
                this.glow.Expand();
            else
                this.glow.Collapse();

            float target = this.itemCount.IsCompleted ? 1 : 0;
            if (this.itemCount.IsEstimate && this.itemCount.PickedUp > 0)
                target = this.itemCount.PickedUp / this.itemCount.TargetCount;

            if (time is null)
            {
                //skip lerping 
                this.glow.LerpToBrightness(target);
            }
            else
            {
                float brightness = MathHelper.Lerp(this.glow.Brightness, target, (float)(10 * time.Delta));
                this.glow.LerpToBrightness(brightness);
            }   
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.ItemName = ParseAttribute(node, "id", string.Empty);
            this.scale = ParseAttribute(node, "scale", this.scale);
        }

        public override void DrawThis(Display display)
        {
            display.Draw(FRAME_INCOMPLETE, this.frame.Bounds, Color.White);
            display.Draw(FRAME_COMPLETE, this.frame.Bounds, Color.White * this.glow.Brightness);
        }
    }
}

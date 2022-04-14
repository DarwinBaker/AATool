using System;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIItemCount : UIControl
    {
        private const string FRAME_INCOMPLETE = "frame_count_incomplete";
        private const string FRAME_COMPLETE   = "frame_count_complete";

        public string ItemName;

        private Pickup itemStat;
        private UIPicture frame;
        private UIPicture icon;
        private UIGlowEffect glow;
        private UITextBlock label;
        private int scale;
        private bool isMainWindow;

        public bool IsCompleted => this.itemStat?.CompletedByAnyone() ?? false;

        public UIItemCount() 
        {
            this.BuildFromTemplate();
            this.scale = 2;
        }

        public UIItemCount(int scale = 2) : this()
        {
            this.scale = scale;
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.isMainWindow = screen is UIMainScreen;

            this.glow  = this.First<UIGlowEffect>();

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
            if (Tracker.TryGetPickup(this.ItemName, out this.itemStat) && this.icon != null)
            {
                this.icon.FlexWidth *= this.scale;
                this.icon.FlexHeight *= this.scale;
                this.icon.SetTexture(this.itemStat.Icon);
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
        }

        protected override void UpdateThis(Time time)
        {
            Tracker.TryGetPickup(this.ItemName, out this.itemStat);

            //update count display
            if (this.itemStat is not null)
            {
                this.UpdateGlowBrightness(time);

                if (this.itemStat.CompletedByAnyone())
                    this.frame.SetTexture(FRAME_COMPLETE);
                else
                    this.frame.SetTexture(FRAME_INCOMPLETE);

                this.icon?.SetTexture(this.itemStat.Icon);
                int percent = (int)Math.Round((float)this.itemStat.PickedUp / this.itemStat.TargetCount * 100);
                if (Config.Main.CompactMode && this.scale is 2)
                { 
                    if (this.itemStat.TargetCount is 1)
                        this.label?.SetText(this.itemStat.PickedUp.ToString());
                    else if (!this.itemStat.IsEstimate)
                        this.label?.SetText(this.itemStat.PickedUp + " / " + this.itemStat.TargetCount);
                    else if (percent == 0)
                        this.label?.SetText(Math.Min(percent, 100) + "%");
                    else
                        this.label?.SetText("~" + Math.Min(percent, 100) + "%");
                }                   
                else if (this.itemStat.TargetCount is 1)
                    this.label?.SetText(this.itemStat.Name);
                else if (!this.itemStat.IsEstimate)
                    this.label?.SetText(this.itemStat.Name + "\n" + this.itemStat.PickedUp + "\0/\0" + this.itemStat.TargetCount);
                else if (percent == 0)
                    this.label?.SetText(this.itemStat.Name + "\n" + Math.Min(percent, 100) + "%");
                else
                    this.label?.SetText(this.itemStat.Name + "\n~" + Math.Min(percent, 100) + "%");
            }
        }

        private void UpdateGlowBrightness(Time time)
        {
            if (this.itemStat is null)
                return;

            if (this.isMainWindow && Config.Main.ShowCompletionGlow)
                this.glow.Expand();
            else
                this.glow.Collapse();

            float target = this.itemStat.IsComplete() ? 1 : 0;
            if (this.itemStat.IsEstimate && this.itemStat.PickedUp > 0)
                target = this.itemStat.PickedUp / this.itemStat.TargetCount;

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
            this.ItemName = Attribute(node, "id", string.Empty);
            this.scale = Attribute(node, "scale", this.scale);
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            canvas.Draw(FRAME_INCOMPLETE, this.frame.Bounds, Color.White);
            canvas.Draw(FRAME_COMPLETE, this.frame.Bounds, Color.White * this.glow.Brightness);
        }
    }
}

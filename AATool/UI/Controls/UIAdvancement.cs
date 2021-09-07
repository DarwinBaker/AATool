using AATool.Data;
using AATool.Graphics;
using AATool.Net;
using AATool.Settings;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIAdvancement : UIControl
    {
        private static readonly Dictionary<FrameType, string> CompleteFrames = new () {
            { FrameType.Normal,     "frame_normal_complete"},
            { FrameType.Goal,       "frame_goal_complete"},
            { FrameType.Challenge,  "frame_challenge_complete"}
        };
        private static readonly Dictionary<FrameType, string> IncompleteFrames = new () {
            { FrameType.Normal,     "frame_normal_incomplete"},
            { FrameType.Goal,       "frame_goal_incomplete"},
            { FrameType.Challenge,  "frame_challenge_incomplete"}
        };

        private const float ALPHA_LOCKED_FRAME = 0.25f;
        private const float ALPHA_LOCKED_ICON  = 0.1f;
        private const float ALPHA_LOCKED_TEXT  = 0.6f;

        private string completeFrame;
        private string incompleteFrame;

        public Advancement Advancement { get; private set; }
        public string AdvancementName;

        private UIControl frame;
        private UIPicture icon;
        private UIGlowEffect glow;
        private UITextBlock label;
        private int scale;
        private bool isMainWindow;

        public bool IsCompleted => this.Advancement?.CompletedByAnyone() ?? false;
        public Point IconCenter => this.icon?.Center ?? Point.Zero;

        public UIAdvancement() 
        {
            this.scale = 2;
            this.BuildFromSourceDocument();
        }

        public UIAdvancement(int scale = 2) : this()
        {
            this.scale = scale;
        }

        public void ShowText() => this.label.Expand();
        public void HideText() => this.label.Collapse();

        public override void InitializeRecursive(UIScreen screen)
        {
            this.isMainWindow    = screen is UIMainScreen;

            Tracker.TryGetAdvancement(this.AdvancementName, out Advancement advancement);
            this.Advancement = advancement;

            this.incompleteFrame = IncompleteFrames[advancement.Type];
            this.completeFrame   = CompleteFrames[advancement.Type];

            int textScale = this.scale < 3 ? 1 : 2;
            this.FlexWidth *= Math.Min(this.scale + textScale - 1, 4);

            this.Padding = Config.IsPostExplorationUpdate 
                ? new Margin(0, 0, 4 * this.scale, 0) 
                : new Margin(0, 0, 6, 0);

            this.icon  = this.First<UIPicture>("icon");
            this.frame = this.First("frame");
            this.glow  = this.First<UIGlowEffect>();
            this.label = this.First<UITextBlock>("label");
            
            this.frame.FlexWidth  *= this.scale;
            this.frame.FlexHeight *= this.scale;
            this.icon.FlexWidth   *= this.scale;
            this.icon.FlexHeight  *= this.scale;

            this.Name = this.Advancement.Id;
            this.icon.SetTexture(this.Advancement.Icon);
            this.icon.SetLayer(Layer.Fore);

            int textSize = this.isMainWindow
                ? 12
                : 12 * ((Config.Overlay.Scale + 1) / 2);

            if (this.label is not null)
            {
                this.label.Margin = new Margin(0, 0, 26 * this.scale, 0);
                if (this.isMainWindow)
                {
                    this.label.FlexHeight = Config.Main.CompactMode
                        ? new Size(textSize)
                        : new Size(textSize * 2);
                }
                else
                {
                    this.label.FlexHeight = new Size(textSize * 2);
                }
                if (this.isMainWindow)
                    this.label.SetFont("minecraft", 12);
                else
                    this.label.SetFont("minecraft", textSize);

                this.label.SetText(this.Advancement.Name);

                if (!Config.IsPostExplorationUpdate && screen is UIMainScreen)
                    this.label.DrawBackground = true;
            }

            if (this.isMainWindow && Config.Main.CompactMode)
            {
                //compact mode
                this.FlexWidth  = new Size(66);
                this.FlexHeight = new Size(68);
                this.label.SetText(this.Advancement.ShortName);
            }
            else
            {
                //relaxed mode
                this.FlexHeight *= this.scale;
            }

            this.UpdateGlowBrightness(null); 
            base.InitializeRecursive(screen);
        }

        private void UpdateGlowBrightness(Time time)
        {
            if (this.isMainWindow && Config.Main.CompletionGlow)
                this.glow.Expand();
            else
                this.glow.Collapse();

            float target = this.IsCompleted ? 1f : 0f;
            if (time is not null)
            {
                float brightness = MathHelper.Lerp(this.glow.Brightness, target, (float)(10 * time.Delta));
                this.glow.LerpToBrightness(brightness);
            }
            else
            {
                this.glow.LerpToBrightness(target);
            }
        }

        public override void UpdateRecursive(Time time)
        {
            if (this.Parent is not UICriteriaGroup)
            {
                if (Config.Main.HideCompleted && this.IsCompleted)
                {
                    this.glow.SkipToBrightness(0);
                    this.Collapse();
                }
                else
                {
                    this.Expand();
                }
            }
            base.UpdateRecursive(time);
        }

        protected override void UpdateThis(Time time)
        {
            this.UpdateGlowBrightness(time);
            base.UpdateThis(time);
        }

        public override void DrawThis(Display display)
        {
            bool grayOut = this.Advancement is Achievement achievement && achievement.IsLocked;
            if (this.isMainWindow && grayOut)
            {
                //achievement is locked. gray it out
                this.icon.SetTint(Color.Gray * ALPHA_LOCKED_ICON);
                this.label?.SetTextColor(MainSettings.Instance.TextColor * ALPHA_LOCKED_TEXT);
                display.Draw(this.incompleteFrame, this.frame.Bounds, Color.Gray * ALPHA_LOCKED_FRAME);
            }
            else 
            {
                //normal advancement rendering
                this.icon.SetTint(Color.White);
                if (this.isMainWindow)
                    this.label?.SetTextColor(Config.Main.TextColor);
                else
                    this.label?.SetTextColor(Config.Overlay.TextColor);

                display.Draw(this.incompleteFrame, this.frame.Bounds, Color.White);
                display.Draw(this.completeFrame, this.frame.Bounds, ColorHelper.Fade(Color.White, this.glow.Brightness));
            }
        }

        public override void DrawRecursive(Display display)
        {
            if (this.IsCollapsed)
                return;

            base.DrawRecursive(display);

            //draw player head if multiple players have save data
            if (this.IsCompleted && (Tracker.Progress.Players.Count > 1 || Peer.IsConnected))
            {
                int x = this.frame.Left - (4 * this.scale);
                int y = this.frame.Top - (3 * this.scale);
                int size = 18 * this.scale;
                var frameRectangle = new Rectangle(x, y, size, size);

                display.Draw(this.completeFrame + "_head", frameRectangle);
                var headRectangle = new Rectangle(this.frame.Left, this.frame.Top + this.scale, 8 * this.scale, 8 * this.scale);
                display.Draw(this.Advancement.FirstCompletionist.ToString(), headRectangle, Color.White, Layer.Fore);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.AdvancementName = ParseAttribute(node, "id", string.Empty);
            this.scale = ParseAttribute(node, "scale", this.scale);
        }
    }
}

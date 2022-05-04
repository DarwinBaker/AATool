using System;
using System.Collections.Generic;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Data.Objectives.Pickups;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIObjectiveFrame : UIControl
    {
        private static readonly Dictionary<FrameType, string> CompleteMinecraftFrames = new () {
            { FrameType.Normal,     "frame_mc_normal_complete"},
            { FrameType.Goal,       "frame_mc_goal_complete"},
            { FrameType.Challenge,  "frame_mc_challenge_complete"},
            { FrameType.Statistic,  "frame_mc_statistic_complete"},
        };
        private static readonly Dictionary<FrameType, string> IncompleteMinecraftFrames = new () {
            { FrameType.Normal,     "frame_mc_normal_incomplete"},
            { FrameType.Goal,       "frame_mc_goal_incomplete"},
            { FrameType.Challenge,  "frame_mc_challenge_incomplete"},
            { FrameType.Statistic,  "frame_mc_statistic_incomplete"},
        };

        public Objective Objective { get; private set; }
        public string ObjectiveId { get; private set; }
        public string ObjectiveOwnerId { get; private set; }
        public bool IsActive { get; private set; }
        public float OverlayCoverPosition { get; set; }

        private UIControl frame;
        private UIPicture icon;
        private UIGlowEffect glow;
        private UITextBlock label;
        private Type objectiveType;
        private Rectangle portraitRectangle;
        private Rectangle avatarRectangle;
        private Rectangle detailRectangle;
        private Rectangle detailRectangleSmall;
        private int scale;
        private string completeFrame;
        private string incompleteFrame;

        public bool ObjectiveCompleted => this.Objective?.IsComplete() is true;
        public Point IconCenter => this.icon.Center;

        public UIObjectiveFrame() 
        {
            this.scale = 2;
            this.BuildFromTemplate();
        }

        public UIObjectiveFrame(Objective objective, int scale = 2)
        {
            this.SetObjective(objective);
            this.scale = scale;
            this.BuildFromTemplate();
        }

        public void ShowText() => this.label.Expand();
        public void HideText() => this.label.Collapse();

        public void SetObjective(Objective objective)
        {
            this.objectiveType = objective?.GetType();
            this.ObjectiveOwnerId = objective is Criterion criterion
                ? criterion.OwnerId
                : string.Empty;

            this.Objective = objective;
            this.ObjectiveId = objective?.Id;
            this.completeFrame = CompleteMinecraftFrames[this.Objective?.Frame ?? FrameType.Normal];
            this.incompleteFrame = IncompleteMinecraftFrames[this.Objective?.Frame ?? FrameType.Normal];
        }

        public void AutoSetObjective()
        {
            if (this.objectiveType == typeof(Advancement))
            {
                if (Tracker.TryGetAdvancement(this.ObjectiveId, out Advancement objective))
                    this.SetObjective(objective);
            }
            else if (this.objectiveType == typeof(Criterion))
            {
                if (Tracker.TryGetCriterion(this.ObjectiveOwnerId, this.ObjectiveId, out Criterion criterion))
                    this.SetObjective(criterion);
            }
            else if (this.objectiveType == typeof(Pickup))
            {
                if (Tracker.TryGetPickup(this.ObjectiveId, out Pickup pickup))
                    this.SetObjective(pickup);
            }
            else if (this.objectiveType == typeof(Block))
            {
                if (Tracker.TryGetBlock(this.ObjectiveId, out Block block))
                    this.SetObjective(block);
            }
            else if (this.objectiveType == typeof(Death))
            {
                if (Tracker.TryGetDeath(this.ObjectiveId, out Death death))
                    this.SetObjective(death);
            }
        }

        public override void InitializeThis(UIScreen screen)
        {
            if (this.Objective is null)
                this.AutoSetObjective();
                
            int textScale = this.scale < 3 ? 1 : 2;
            this.FlexWidth *= Math.Min(this.scale + textScale - 1, 4);

            this.Padding = Tracker.Category is AllAchievements
                ? new Margin(0, 0, 6, 0)
                : new Margin(0, 0, 4 * this.scale, 0);

            this.icon  = this.First<UIPicture>("icon");
            this.frame = this.First("frame");
            this.glow  = this.First<UIGlowEffect>();
            this.label = this.First<UITextBlock>("label");
            
            this.frame.FlexWidth  *= this.scale;
            this.frame.FlexHeight *= this.scale;
            this.icon.FlexWidth   *= this.scale;
            this.icon.FlexHeight  *= this.scale;

            this.Name = this.Objective?.Id;
            this.icon.SetTexture(this.Objective?.Icon);
            this.icon.SetLayer(Layer.Fore);

            //set up label
            if (this.label is not null)
            {
                this.label.Margin = new Margin(0, 0, 26 * this.scale, 0);

                int textSize = screen is UIMainScreen ? 12 : 24;
                if (screen is UIMainScreen)
                {
                    this.label.FlexHeight = Config.Main.CompactMode
                        ? new Size(textSize)
                        : new Size(textSize * 2);
                }
                else
                {
                    this.label.FlexHeight = new Size(textSize * 2);
                }
                if (screen is UIMainScreen)
                    this.label.SetFont("minecraft", 12);
                else
                    this.label.SetFont("minecraft", textSize);

                if (Tracker.Category is AllAchievements && screen is UIMainScreen)
                    this.label.DrawBackground = true;
            }

            if (screen is UIMainScreen && Config.Main.CompactMode && Tracker.Category is not (SingleAdvancement or AllBlocks))
            {
                //make gap between frames slightly smaller in compact mode
                this.FlexWidth  = new Size(66);
                this.FlexHeight = new Size(68);
                this.label?.SetText(this.Objective?.ShortName);
            }
            else
            {
                //relaxed mode
                this.FlexHeight *= this.scale;
                this.label?.SetText(this.Objective?.Name);
            }
            this.UpdateGlowBrightness(null); 
        }

        public override void ResizeRecursive(Rectangle parent) 
        {
            base.ResizeRecursive(parent);

            int x = this.frame.Left - (4 * this.scale);
            int y = this.frame.Top - (3 * this.scale);
            int size = 18 * this.scale;
            this.portraitRectangle = new Rectangle(x, y, size, size);

            this.avatarRectangle = new Rectangle(
                this.frame.Left, 
                this.frame.Top + this.scale, 
                8 * this.scale, 
                8 * this.scale);

            this.detailRectangle = new Rectangle(
                this.frame.Left + 2 * this.scale,
                this.frame.Top + 2 * this.scale,
                this.frame.Width - 4 * this.scale,
                this.frame.Height - 4 * this.scale);

            this.detailRectangleSmall = new Rectangle(
                this.frame.Left + 4 * this.scale,
                this.frame.Top + 4 * this.scale,
                this.frame.Width - 8 * this.scale,
                this.frame.Height - 8 * this.scale);
        }

        private void UpdateAppearance()
        {
            if (this.Root() is not UIOverlayScreen)
                this.label?.SetTextColor(this.IsActive ? Config.Main.TextColor : Config.Main.TextColor.Value * 0.4f);
            this.icon?.SetTint(this.IsActive ? Color.White : ColorHelper.Fade(Color.DarkGray, 0.1f));

            if (this.Objective is Trident trident)
                this.icon?.SetTexture(trident.Icon);
        }

        private void UpdateGlowBrightness(Time time)
        {
            if (this.glow is null)
                return;

            if (Config.Main.ShowCompletionGlow && this.Root() is not UIOverlayScreen)
                this.glow.Expand();
            else
                this.glow.Collapse();

            float current = this.glow.Brightness;
            float target = this.Root() is not UIOverlayScreen && Config.Main.FrameStyle == "Modern" 
                ? 0.15f
                : 0;

            if (this.Objective?.IsComplete() is true)
            {
                target = 1;
            }
            else if (Tracker.Category is HalfPercent && this.Objective is Advancement adv && !adv.UsedInHalfPercent)
            {
                target = 0;
            }

            if (Config.Main.FrameStyle == "None")
                target /= 1.25f;

            if (time is not null)
            {
                if (Math.Abs(current - target) > 0.01f && this.Root() is UIMainScreen)
                    UIMainScreen.Invalidate();
                float smoothed = MathHelper.Lerp(this.glow.Brightness, target, (float)(10 * time.Delta));
                this.glow.LerpToBrightness(smoothed);
            }
            else
            {
                if (Math.Abs(current - target) > 0.01f && this.Root() is UIMainScreen)
                    UIMainScreen.Invalidate();
                this.glow.LerpToBrightness(target);
            }
        }

        private void UpdateActiveState()
        {
            this.IsActive = true;
            if (this.Objective is Advancement adv && !this.Objective?.IsComplete() is true)
            {
                this.IsActive &= !(adv is Achievement ach && ach.IsLocked);
                this.IsActive &= !(Tracker.Category is HalfPercent && !adv.UsedInHalfPercent);
            }
            this.IsActive |= this.Root() is UIOverlayScreen;
        }

        public override void UpdateRecursive(Time time)
        {
            if (this.Parent is not UICriteriaGroup)
            {
                if (Config.Main.HideCompletedAdvancements
                    && this.Objective?.IsComplete() is true 
                    && this.Objective is Advancement)
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
            this.UpdateActiveState();
            this.UpdateGlowBrightness(time);
            this.UpdateAppearance();

            //pickups have labels that change over time and need to be refreshed
            if (this.Objective is Pickup)
            {
                bool fullSize = Config.Main.RelaxedMode && Tracker.Category is AllAdvancements or AllAchievements or AllBlocks;
                if (fullSize || this.Root() is UIOverlayScreen)
                    this.label?.SetText(this.Objective?.GetFullCaption());
                else
                    this.label?.SetText(this.Objective?.GetShortCaption());
            }

            //uncomment for making preview images easily
            //this.label?.SetText(Config.Overlay.FrameStyle);
        }

        public override void DrawThis(Canvas canvas)
        {
            bool overlay = this.Root() is UIOverlayScreen;
            float opacity = overlay ? 1 : (this.IsActive ? 0.7f : 0.1f);
            string style = overlay ? Config.Overlay.FrameStyle : Config.Main.FrameStyle;

            if (style is "Furnace" or "Blazed")
            {
                Color tint = ColorHelper.Fade(Color.White, this.glow.Brightness / (overlay ? 2f : 3f));
                canvas.Draw($"fire_flat", this.detailRectangle, tint, Layer.Glow);
            }
            else if (overlay && style is "Eye Spy")
            {
                Color tint = ColorHelper.Fade(Color.White, this.glow.Brightness / 2f);
                canvas.Draw($"fire_flat", this.detailRectangleSmall, tint, Layer.Glow);
            }
            if (this.SkipDraw || this.Objective is null || style is "None")
                return;

            switch (style.ToLower())
            {
                case "minecraft":
                    canvas.Draw(this.incompleteFrame, this.frame.Bounds, this.IsActive ? Color.White : Color.Gray * 0.25f);
                    canvas.Draw(this.completeFrame, this.frame.Bounds, ColorHelper.Fade(Color.White, this.glow.Brightness));
                    break;
                case "furnace":
                case "blazed":
                    canvas.Draw("frame_modern_back", this.frame.Bounds, this.Root().FrameBackColor() * opacity);
                    if (this.IsActive)
                    {
                        Color brightness = ColorHelper.Fade(Color.White, this.glow.Brightness);
                        canvas.Draw("frame_furnace_back_complete", this.frame.Bounds, brightness);
                        canvas.Draw("frame_modern_border", this.frame.Bounds, this.Root().FrameBorderColor());
                        canvas.Draw("frame_modern_border_complete", this.frame.Bounds, brightness);
                    }
                    break;
                case "geode":
                    canvas.Draw("frame_geode_back", this.frame.Bounds, Color.White);
                    if (this.IsActive)
                    {
                        Color brightness = ColorHelper.Fade(Color.White, this.glow.Brightness);
                        canvas.Draw("frame_geode_back_complete", this.frame.Bounds, brightness);
                        canvas.Draw("frame_geode_border", this.frame.Bounds, Color.White);
                        canvas.Draw("frame_geode_border_complete", this.frame.Bounds, brightness);
                    }
                    else
                    {
                        canvas.Draw("frame_geode_border", this.frame.Bounds, Color.White * 0.75f);
                    }
                    break;
                case "eye spy":
                    canvas.Draw("frame_eyespy", this.frame.Bounds, this.IsActive ? Color.White : Color.Gray * 0.25f);
                    if (this.IsActive)
                    {
                        Color brightness = ColorHelper.Fade(Color.White, this.glow.Brightness);
                        canvas.Draw("frame_eyespy_complete", this.frame.Bounds, brightness);
                    }
                    break;
                case "flex":
                    canvas.Draw("frame_flex_incomplete", this.frame.Bounds, this.IsActive ? Color.White : Color.Gray * 0.25f);
                    canvas.Draw("frame_flex_complete", this.frame.Bounds, ColorHelper.Fade(Color.White, this.glow.Brightness));
                    break;
                case "enchanted":
                    canvas.Draw("frame_enchanted_incomplete", this.frame.Bounds, this.IsActive ? Color.White : Color.Gray * 0.25f);
                    canvas.Draw("frame_enchanted_complete", this.frame.Bounds, ColorHelper.Fade(Color.White, this.glow.Brightness));
                    break;
                default: 
                    canvas.Draw("frame_modern_back", this.frame.Bounds, this.Root().FrameBackColor() * opacity);
                    if (this.IsActive)
                    {
                        Color brightness = ColorHelper.Fade(Color.White, this.glow.Brightness);
                        canvas.Draw("frame_modern_back_complete", this.frame.Bounds, brightness);
                        canvas.Draw("frame_modern_border", this.frame.Bounds, this.Root().FrameBorderColor());
                        canvas.Draw("frame_modern_border_complete", this.frame.Bounds, brightness);
                    }
                    break;
            }
        }

        public override void DrawRecursive(Canvas canvas)
        {
            if (this.IsCollapsed)
                return;

            base.DrawRecursive(canvas);

            //skip drawing player heads if in solo filter mode
            if (Config.Tracking.Filter == ProgressFilter.Solo && !Peer.IsRunning)
                return;

            //draw player head if multiple players have save data
            if (this.ObjectiveCompleted 
                && this.Objective is Advancement 
                && (Tracker.State.Players.Count > 1 || Peer.IsConnected))
            {
                Color fade = ColorHelper.Fade(Color.White, this.glow.Brightness);
                if (!this.SkipDraw)
                {
                    switch (Config.Main.FrameStyle)
                    {
                        case "Minecraft":
                            canvas.Draw(this.completeFrame + "_portrait", this.portraitRectangle, fade);
                            break;
                        case "Modern":
                            canvas.Draw("frame_modern_portrait", this.portraitRectangle, fade);
                            break;
                        case "Furnace":
                        case "Blazed":
                            canvas.Draw("frame_furnace_portrait", this.portraitRectangle, fade);
                            break;
                        case "Geode":
                            canvas.Draw("frame_geode_portrait", this.portraitRectangle, fade);
                            break;
                        case "Eye Spy":
                            canvas.Draw("frame_eyespy_portrait", this.portraitRectangle, fade);
                            break;
                        case "Flex":
                            canvas.Draw("frame_flex_portrait", this.portraitRectangle, fade);
                            break;
                    }
                }
                canvas.Draw($"avatar-{this.Objective.FirstCompletion.who}",
                    this.avatarRectangle, fade, Layer.Fore);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.scale = Attribute(node, "scale", this.scale);

            //check if this frame contains an advancement
            this.ObjectiveId = Attribute(node, "advancement", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.objectiveType = typeof(Advancement);
                return;
            }

            //check if this frame contains an achievement
            this.ObjectiveId = Attribute(node, "achievement", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.objectiveType = typeof(Advancement);
                return;
            }

            //check if this frame contains a criterion
            this.ObjectiveId = Attribute(node, "criterion", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.objectiveType = typeof(Criterion);
                this.ObjectiveOwnerId = Attribute(node, "owner", string.Empty);
                return;
            }

            //check if this frame contains a pickup counter
            this.ObjectiveId = Attribute(node, "pickup", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.objectiveType = typeof(Pickup);
                return;
            }

            //check if this frame contains a block
            this.ObjectiveId = Attribute(node, "block", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.objectiveType = typeof(Block);
                return;
            }

            //check if this frame contains a death message
            this.ObjectiveId = Attribute(node, "death", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.objectiveType = typeof(Death);
                return;
            }
        }
    }
}

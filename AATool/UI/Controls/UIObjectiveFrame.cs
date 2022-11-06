using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Data.Objectives.Complex;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIObjectiveFrame : UIControl
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
        private static Color ActiveTint = Color.White;
        private static Color InactiveTint = Color.Gray * 0.25f;
        private static Color InactiveIconTint = ColorHelper.Fade(Color.DarkGray, 0.1f);

        public Objective Objective { get; private set; }
        public string ObjectiveId { get; private set; }
        public string ObjectiveOwnerId { get; private set; }
        public bool IsActive { get; private set; }
        public float OverlayCoverPosition { get; set; }

        public int Scale { get; protected set; }

        public UIControl Frame { get; protected set; }
        public UIPicture Icon { get; protected set; }
        public UIGlowEffect Glow { get; protected set; }
        public UITextBlock Label { get; protected set; }
        public UIButton ManualCheck { get; protected set; }

        private Type objectiveType;
        private Rectangle portraitRectangle;
        private Rectangle avatarRectangle;
        private Rectangle detailRectangle;
        private Rectangle detailRectangleSmall;
        private string style;
        private string flag;
        private string completeFrame;
        private string incompleteFrame;
        private bool onMainScreen;

        public bool ObjectiveCompleted => this.Objective?.IsComplete() is true;
        public Point IconCenter => this.Icon.Center;

        public UIObjectiveFrame() 
        {
            this.Scale = 2;
            this.BuildFromTemplate();
        }

        public UIObjectiveFrame(Objective objective, int scale = 2)
        {
            this.SetObjective(objective);
            this.Scale = scale;
            this.BuildFromTemplate();
        }

        public void ShowText() => this.Label.Expand();
        public void HideText() => this.Label.Collapse();

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
            else if (this.objectiveType == typeof(ComplexObjective))
            {
                if (Tracker.TryGetComplexObjective(this.ObjectiveId, out ComplexObjective objective))
                    this.SetObjective(objective);
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

        private void UpdateManualCheckbox()
        {
            if (this.ManualCheck is null || this.Objective is null)
                return;

            if (Tracker.IsWorking && this.Objective.CanBeManuallyChecked)
            {
                this.ManualCheck.Expand();
                string variant = Config.Main.FrameStyle == "Modern" ? "modern" : "basic";
                variant = this.Objective.IsComplete() ? $"checked_{variant}" : $"unchecked_{variant}";
                this.ManualCheck.First<UIPicture>()?.SetTexture(variant);
            }
            else
            {
                this.ManualCheck.Collapse();       
            }
        }

        public override void InitializeThis(UIScreen screen)
        {
            if (this.Objective is null)
                this.AutoSetObjective();
                
            int textScale = this.Scale < 3 ? 1 : 2;
            this.FlexWidth *= Math.Min(this.Scale + textScale - 1, 4);

            this.Padding = Tracker.Category is AllAchievements
                ? new Margin(0, 0, 6, 0)
                : new Margin(0, 0, 4 * this.Scale, 0);

            this.Icon  = this.First<UIPicture>("icon");
            this.Frame = this.First("frame");
            this.Glow  = this.First<UIGlowEffect>();
            this.Label = this.First<UITextBlock>("label");
            
            this.Frame.FlexWidth  *= this.Scale;
            this.Frame.FlexHeight *= this.Scale;
            this.Icon.FlexWidth   *= this.Scale;
            this.Icon.FlexHeight  *= this.Scale;

            this.Name = this.Objective?.Id;
            this.Icon.SetTexture(this.Objective?.Icon);
            this.Icon.SetLayer(Layer.Fore);

            this.onMainScreen = screen is UIMainScreen mainScreen;

            //set up label
            if (this.Label is not null)
            {
                this.Label.Margin = new Margin(0, 0, 26 * this.Scale, 0);

                int textSize = this.onMainScreen ? 12 : 24;
                if (this.onMainScreen)
                {
                    this.Label.FlexHeight = Config.Main.UseCompactStyling
                        ? new Size(textSize)
                        : new Size(textSize * 2);
                }
                else
                {
                    this.Label.FlexHeight = new Size(textSize * 2);
                }
                if (this.onMainScreen)
                    this.Label.SetFont("minecraft", 12);
                else
                    this.Label.SetFont("minecraft", textSize);

                if (Tracker.Category is AllAchievements && this.onMainScreen)
                    this.Label.DrawBackground = true;
            }

            if (this.onMainScreen && Config.Main.UseCompactStyling && Tracker.Category is not (SingleAdvancement or AllBlocks))
            {
                //make gap between frames slightly smaller in compact mode
                this.FlexWidth  = new Size(66);
                this.FlexHeight = new Size(68);
                this.Label?.SetText(this.Objective?.ShortName);
            }
            else
            {
                //relaxed mode
                this.FlexHeight *= this.Scale;
                this.Label?.SetText(this.Objective?.Name);
            }

            //add manual override checkbox
            if (this.Objective is not null && this.Objective.CanBeManuallyChecked && this.onMainScreen)
            {
                Margin margin = this.Objective is Death
                    ? new Margin(0, -2, -10, 0)
                    : new Margin(0, 5, -3, 0);

                string id = this.Objective is ComplexObjective
                    ? this.Objective.Name : this.Objective.Id;

                this.ManualCheck = new UIButton() {
                    FlexWidth = new(20),
                    FlexHeight = new(20),
                    Margin = margin,
                    VerticalAlign = VerticalAlign.Top,
                    HorizontalAlign = HorizontalAlign.Right,
                    BorderThickness = 2,
                    Name = "manual_check",
                    Tag = id,
                    Layer = Layer.Main,
                };
                this.ManualCheck.AddControl(new UIPicture());
                this.ManualCheck.OnClick += (this.Root() as UIMainScreen).Click;
                this.AddControl(this.ManualCheck);
            }

            this.UpdateGlowBrightness(null);

            if (!this.onMainScreen)
                this.UpdateAppearance(true);
        }

        public override void ResizeRecursive(Rectangle parent) 
        {
            base.ResizeRecursive(parent);

            int x = this.Frame.Left - (4 * this.Scale);
            int y = this.Frame.Top - (3 * this.Scale);
            int size = 18 * this.Scale;
            this.portraitRectangle = new Rectangle(x, y, size, size);

            this.avatarRectangle = new Rectangle(
                this.Frame.Left, 
                this.Frame.Top + this.Scale, 
                8 * this.Scale, 
                8 * this.Scale);

            this.detailRectangle = new Rectangle(
                this.Frame.Left + 2 * this.Scale,
                this.Frame.Top + 2 * this.Scale,
                this.Frame.Width - 4 * this.Scale,
                this.Frame.Height - 4 * this.Scale);

            this.detailRectangleSmall = new Rectangle(
                this.Frame.Left + 4 * this.Scale,
                this.Frame.Top + 4 * this.Scale,
                this.Frame.Width - 8 * this.Scale,
                this.Frame.Height - 8 * this.Scale);

            if (this.onMainScreen)
                this.UpdateAppearance(true);
            else
                this.Glow?.Collapse();
        }

        private void UpdateAppearance(bool forceUpdate = false)
        {
            if (this.onMainScreen)
            {
                this.Label?.SetTextColor(this.IsActive ? Config.Main.TextColor : Config.Main.TextColor.Value * 0.4f);
            }
            else
            {
                this.Label?.SetVisibility(this.Objective is ComplexObjective || (!this.ObjectiveCompleted && Config.Overlay.ShowLabels));
            }
            this.Icon?.SetTint(this.IsActive ? Color.White : InactiveIconTint);

            bool invalidated = this.onMainScreen 
                ? Config.Main.AppearanceChanged
                : Config.Overlay.AppearanceChanged;

            if (invalidated || forceUpdate)
            {
                this.style = this.onMainScreen 
                    ? Config.Main.GetActiveFrameStyle(this.Center.X, this.Center.Y) 
                    : Config.Overlay.GetActiveFrameStyle(this.style);

                this.style = this.style.ToLower();
                if (this.style.Contains("pride"))
                {
                    this.flag = $"frame_flag_{this.style.Split(' ').First()}";
                    this.style = "flag";
                }
                else
                {
                    this.flag = string.Empty;
                }
            }
        }

        private void UpdateGlowBrightness(Time time)
        {
            if (this.Glow is null)
                return;

            float current = this.Glow.Brightness;
            float target = this.onMainScreen && Config.Main.FrameStyle == "Modern" 
                ? 0.15f : 0;

            if (this.Objective?.IsComplete() is true)
                target = 1;
            else if (Tracker.Category is HalfPercent && this.Objective is Advancement adv && !adv.UsedInHalfPercent)
                target = 0;

            if (this.style == "none")
                target /= 1.25f;

            if (time is not null)
            {
                if (Math.Abs(current - target) > 0.01f && this.onMainScreen)
                    UIMainScreen.Invalidate();
                float smoothed = MathHelper.Lerp(this.Glow.Brightness, target, (float)(10 * time.Delta));
                this.Glow.LerpToBrightness(smoothed);
            }
            else
            {
                if (Math.Abs(current - target) > 0.01f && this.onMainScreen)
                    UIMainScreen.Invalidate();
                this.Glow.LerpToBrightness(target);
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
                    this.Glow?.SkipToBrightness(0);
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
            this.UpdateManualCheckbox();

            //pickups have labels that change over time and need to be refreshed
            if (this.Objective is ComplexObjective || Tracker.Invalidated || Config.Tracking.FilterChanged)
            {
                bool fullSize = !Config.Main.UseCompactStyling && Tracker.Category is AllAdvancements or AllAchievements or AllBlocks;
                if (fullSize || this.Root() is UIOverlayScreen)
                    this.Label?.SetText(this.Objective?.FullStatus);
                else
                    this.Label?.SetText(this.Objective?.TinyStatus);
                this.Icon?.SetTexture(this.Objective?.Icon);
            }

            //uncomment for making preview images easily
            //this.label?.SetText(Config.Overlay.FrameStyle);
        }

        public override void DrawThis(Canvas canvas)
        {
            bool overlay = this.Root() is UIOverlayScreen;
            float opacity = overlay ? 1 : (this.IsActive ? 0.7f : 0.1f);

            if (this.style is "furnace" or "blazed")
            {
                Color tint = ColorHelper.Fade(Color.White, this.Glow.Brightness / (overlay ? 2f : 3f));
                canvas.Draw($"fire_flat", this.detailRectangle, tint, Layer.Glow);
            }
            else if (overlay && this.style is "eye spy")
            {
                Color tint = ColorHelper.Fade(Color.White, this.Glow.Brightness / 2f);
                canvas.Draw($"fire_flat", this.detailRectangleSmall, tint, Layer.Glow);
            }
            if (this.SkipDraw || this.Objective is null || this.style is "none")
                return;

            switch (this.style)
            {
                case "minecraft":
                    canvas.Draw(this.incompleteFrame, this.Frame.Bounds, this.IsActive ? ActiveTint : InactiveTint, this.Layer);
                    canvas.Draw(this.completeFrame, this.Frame.Bounds, ColorHelper.Fade(ActiveTint, this.Glow.Brightness), this.Layer);
                    break;
                case "furnace":
                case "blazed":
                    canvas.Draw("frame_modern_back", this.Frame.Bounds, this.Root().FrameBackColor() * opacity, this.Layer);
                    if (this.IsActive)
                    {
                        Color faded = ColorHelper.Fade(ActiveTint, this.Glow.Brightness);
                        canvas.Draw("frame_furnace_back_complete", this.Frame.Bounds, faded, this.Layer);
                        canvas.Draw("frame_modern_border", this.Frame.Bounds, this.Root().FrameBorderColor(), this.Layer);
                        canvas.Draw("frame_modern_border_complete", this.Frame.Bounds, faded, this.Layer);
                    }
                    break;
                case "geode":
                    canvas.Draw("frame_geode_back", this.Frame.Bounds, ActiveTint);
                    if (this.IsActive)
                    {
                        Color faded = ColorHelper.Fade(ActiveTint, this.Glow.Brightness);
                        canvas.Draw("frame_geode_back_complete", this.Frame.Bounds, faded, this.Layer);
                        canvas.Draw("frame_geode_border", this.Frame.Bounds, ActiveTint, this.Layer);
                        canvas.Draw("frame_geode_border_complete", this.Frame.Bounds, faded, this.Layer);
                    }
                    else
                    {
                        canvas.Draw("frame_geode_border", this.Frame.Bounds, ActiveTint * 0.75f, this.Layer);
                    }
                    break;
                case "eye spy":
                    canvas.Draw("frame_eyespy", this.Frame.Bounds, this.IsActive ? ActiveTint : InactiveTint, this.Layer);
                    if (this.IsActive)
                    {
                        Color faded = ColorHelper.Fade(Color.White, this.Glow.Brightness);
                        canvas.Draw("frame_eyespy_complete", this.Frame.Bounds, faded, this.Layer);
                    }
                    break;
                case "flag":
                    if (this.IsActive)
                    {
                        Color faded = ColorHelper.Fade(ActiveTint, this.Glow.Brightness);
                        canvas.Draw(this.flag, this.detailRectangle, this.IsActive ? ActiveTint : InactiveTint, this.Layer);
                        canvas.Draw("frame_flag_shading", this.detailRectangle, ActiveTint * 0.5f, this.Layer);
                        canvas.Draw("frame_modern_back_complete", this.Frame.Bounds, faded, this.Layer);
                        canvas.Draw("frame_modern_border", this.Frame.Bounds, ActiveTint, this.Layer);
                        canvas.Draw("frame_modern_border_complete", this.Frame.Bounds, faded, this.Layer);
                    }
                    else
                    {
                        canvas.Draw("frame_modern_back", this.Frame.Bounds, this.Root().FrameBackColor() * opacity, this.Layer);
                        canvas.Draw("frame_modern_border", this.Frame.Bounds, InactiveIconTint, this.Layer);
                    }
                    break;
                default: 
                    canvas.Draw("frame_modern_back", this.Frame.Bounds, this.Root().FrameBackColor() * opacity, this.Layer);
                    if (this.IsActive)
                    {
                        Color faded = ColorHelper.Fade(Color.White, this.Glow.Brightness);
                        canvas.Draw("frame_modern_back_complete", this.Frame.Bounds, faded, this.Layer);
                        canvas.Draw("frame_modern_border", this.Frame.Bounds, this.Root().FrameBorderColor(), this.Layer);
                        canvas.Draw("frame_modern_border_complete", this.Frame.Bounds, faded, this.Layer);
                    }
                    else
                    {
                        canvas.Draw("frame_modern_border", this.Frame.Bounds, InactiveIconTint, this.Layer);
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
                Color fade = ColorHelper.Fade(Color.White, this.Glow.Brightness);
                if (!this.SkipDraw)
                {
                    switch (this.style)
                    {
                        case "minecraft":
                            canvas.Draw(this.completeFrame + "_portrait", this.portraitRectangle, fade, this.Layer);
                            break;
                        case "modern":
                            canvas.Draw("frame_modern_portrait", this.portraitRectangle, fade, this.Layer);
                            break;
                        case "furnace":
                        case "blazed":
                            canvas.Draw("frame_furnace_portrait", this.portraitRectangle, fade, this.Layer);
                            break;
                        case "geode":
                            canvas.Draw("frame_geode_portrait", this.portraitRectangle, fade, this.Layer);
                            break;
                        case "eye spy":
                            canvas.Draw("frame_eyespy_portrait", this.portraitRectangle, fade, this.Layer);
                            break;
                    }
                }

                if (this.onMainScreen)
                    canvas.Draw($"avatar-{this.Objective.FirstCompletion.Player}", this.avatarRectangle, fade, Layer.Fore);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.Scale = Attribute(node, "scale", this.Scale);

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
            this.ObjectiveId = Attribute(node, "complex", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.objectiveType = typeof(ComplexObjective);
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

        public void SetForeground()
        {
            this.Layer = Layer.Fore;
            this.Frame.Layer = Layer.Fore;
            this.Icon.Layer = Layer.Fore;
            this.Label.Layer = Layer.Fore;
        }
    }
}

using System.Xml;
using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using static AATool.Configuration.Config;

namespace AATool.UI.Controls
{
    class UICriterion : UIObjectiveControl
    {
        public bool IsStatic        { get; set; }

        private readonly int scale;
        private readonly int imageSize;

        private UIPicture icon;
        private UITextBlock label;

        private float iconBrightness;
        private float textBrightness;
        private float iconTarget;
        private float textTarget;

        public bool HideFromOverlay => this.Objective is Criterion crit && (crit.Owner.IsComplete() || crit.CompletedByDesignated());

        public UICriterion() : base()
        {
            this.BuildFromTemplate(); 
            this.imageSize = 16;
            this.scale = 1;
        }

        public UICriterion(int scale = 1) : this()
        {
            this.scale = scale;
            this.imageSize = 16 * scale;
            this.FlexWidth *= scale;
            if (scale > 1)
            {
                this.FlexWidth  = new Size(68, SizeMode.Absolute);
                this.FlexHeight = new Size(68, SizeMode.Absolute);
                this.Padding = new Margin(0, 20, 0, 20);
            }
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            if (this.Objective is null)
                this.AutoSetObjective();


            if (Config.Main.UseCompactStyling && this.scale is 1)
                this.FlexWidth = new Size(95, SizeMode.Absolute);

            this.icon = this.First<UIPicture>("icon");
            if (this.icon is not null && this.Objective is not null)
            {
                this.icon.SetTexture(this.Objective?.Icon);
                this.icon.FlexWidth  = new Size(this.imageSize, SizeMode.Absolute);
                this.icon.FlexHeight = new Size(this.imageSize, SizeMode.Absolute);
            }

            this.label = this.First<UITextBlock>("label");
            if (this.scale is 1)
            { 
                if (Config.Main.UseCompactStyling && !Config.Main.UseOptimizedLayout)
                    this.label?.SetText(this.Objective?.TinyStatus);
                else
                    this.label?.SetText(this.Objective?.FullStatus);
            }    
            else
            {
                this.RemoveControl(this.label);
            }

            if (!this.IsStatic)
            {
                bool completed = this.ObjectiveCompleted;
                if (Config.Main.HideCompletedCriteria)
                {
                    this.iconBrightness = completed ? 0 : 1f;
                    this.textBrightness = completed ? 0 : 1f;
                }
                else
                {
                    this.iconBrightness = completed ? 1f : 0.35f;
                    this.textBrightness = completed ? 1f : 0.5f;
                }
            }
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        { 
            if (this.IsStatic)
                return;

            if ((Tracker.Invalidated || Tracker.DesignationsChanged) && this.scale is 1)
            {
                if (Config.Main.UseCompactStyling && !Config.Main.UseOptimizedLayout)
                    this.label?.SetText(this.Objective?.TinyStatus);
                else
                    this.label?.SetText(this.Objective?.FullStatus);
            }

            bool completed = this.ObjectiveCompleted;
            if (Config.Main.HideCompletedCriteria)
            {
                this.iconTarget = completed ? 0 : 1f;
                this.textTarget = completed ? 0 : 1f;
            }
            else
            {
                this.iconTarget = completed ? 1f : 0.35f;
                this.textTarget = completed ? 1f : 0.5f;
            }

            bool visible = !(completed && Config.Main.HideCompletedCriteria);
            this.icon?.SetVisibility(visible);
            this.label?.SetVisibility(visible);

            this.iconBrightness = MathHelper.Lerp(this.iconBrightness, this.iconTarget, (float)(10 * time.Delta));
            this.icon?.SetTint(Color.White * this.iconBrightness);

            this.textBrightness = MathHelper.Lerp(this.textBrightness, this.textTarget, (float)(10 * time.Delta));
            this.label?.SetTextColor(Config.Main.TextColor.Value * this.textTarget);
        }
    }
}

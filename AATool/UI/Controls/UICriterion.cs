using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UICriterion : UIControl
    {
        public string AdvancementID { get; set; }
        public string CriterionID   { get; set; }
        public bool IsStatic        { get; set; }

        private readonly int scale;
        private readonly int imageSize;
        
        private Criterion criterion;
        private UIPicture icon;
        private UITextBlock label;
        private float iconBrightness;
        private float textBrightness;

        public bool HideFromOverlay => (this.criterion?.Owner?.CompletedByAnyone() ?? false) || this.CriterionCompleted;
        public bool CriterionCompleted => this.criterion?.CompletedByDesignated() ?? false;

        public UICriterion() 
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
            }
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            Tracker.TryGetCriterion(this.AdvancementID, this.CriterionID, out this.criterion);

            if (Config.Main.CompactMode && this.scale is 1)
                this.FlexWidth = new Size(95, SizeMode.Absolute);

            this.icon = this.First<UIPicture>("icon");
            if (this.icon is not null)
            {
                this.icon.SetTexture(this.criterion.Icon);
                this.icon.FlexWidth  = new Size(this.imageSize, SizeMode.Absolute);
                this.icon.FlexHeight = new Size(this.imageSize, SizeMode.Absolute);
            }

            this.label = this.First<UITextBlock>("label");
            if (this.scale is 1)
            { 
                if (Config.Main.CompactMode)
                    this.label?.SetText(this.criterion.ShortName);
                else
                    this.label?.SetText(this.criterion.Name);
            }    
            else
            {
                this.RemoveControl(this.label);
            }

            if (!this.IsStatic)
            {
                float newIconBrightness = this.criterion.CompletedBy(this.criterion.DesignatedPlayer) ? 1f : 0.35f;
                float newTextBrightness = this.criterion.CompletedBy(this.criterion.DesignatedPlayer) ? 1f : 0.5f;
                if (this.iconBrightness != newIconBrightness && this.Root() is UIMainScreen)
                    UIMainScreen.Invalidate();
                if (this.textBrightness != newTextBrightness && this.Root() is UIMainScreen)
                    UIMainScreen.Invalidate();

                this.iconBrightness = newIconBrightness;
                this.textBrightness = newTextBrightness;
            }
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.IsStatic)
            {
                float iconTarget = this.CriterionCompleted ? 1f : 0.35f;
                this.iconBrightness = MathHelper.Lerp(this.iconBrightness, iconTarget, (float)(10 * time.Delta));
                this.icon?.SetTint(Color.White * this.iconBrightness);

                float textTarget = this.CriterionCompleted ? 1f : 0.5f;
                this.textBrightness = MathHelper.Lerp(this.textBrightness, textTarget, (float)(10 * time.Delta));        
                this.label?.SetTextColor(Config.Main.TextColor.Value * textTarget);
            }
        }
    }
}

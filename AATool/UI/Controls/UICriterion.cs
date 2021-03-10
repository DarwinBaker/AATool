using AATool.DataStructures;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UICriterion : UIControl
    {
        public string AdvancementName;
        public string CriterionName;
        public bool IsStatic;

        private Criterion criterion;
        private UIPicture icon;
        private int scale;
        private int imageSize;

        public bool IsCompleted => criterion?.IsCompleted ?? false;

        public UICriterion() 
        {
            InitializeFromSourceDocument();
            scale = 1;
            imageSize = 16 * scale;
        }

        public UICriterion(int scale = 1) : this()
        {
            this.scale = scale;
            imageSize = 16 * scale;
            FlexWidth *= scale;
            if (scale > 1)
            {
                FlexWidth  = new Size(68, SizeMode.Absolute);
                FlexHeight = new Size(68, SizeMode.Absolute);
            }
            Margin = new Margin(8 * scale, 0, 2, 0);
        }

        public override void InitializeRecursive(Screen screen)
        {
            criterion = screen.AdvancementTracker.Advancement(AdvancementName).Criteria.TryGetValue(CriterionName, out var val) ? val : null;
            if (criterion == null)
                return;

            icon = GetControlByName("icon", true) as UIPicture;
            if (icon != null)
            {
                icon.SetTexture(criterion.Icon);
                icon.FlexWidth = new Size(imageSize, SizeMode.Absolute);
                icon.FlexHeight = new Size(imageSize, SizeMode.Absolute);
            }

            var label = GetControlByName("label", true) as UITextBlock;
            if (scale == 1)
                label?.SetText(criterion.Name);
            else
                RemoveControl(label);
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            if (criterion != null)
            {
                if (IsStatic)
                    icon?.SetTint(Color.White);
                else
                    icon?.SetTint(criterion.IsCompleted ? Color.White : Color.White * 0.35f);
            }
        }
    }
}

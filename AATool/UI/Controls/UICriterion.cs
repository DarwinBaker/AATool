using AATool.DataStructures;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System.Xml;

namespace AATool.UI.Controls
{
    class UICriterion : UIPicture
    {
        public string AdvancementName;
        public string CriterionName;
        public bool IsStatic;

        private Criterion criterion;
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
                FlexWidth = new Size(68, SizeMode.Absolute);
            Margin = new Margin(8 * scale, 0, 2, 0);
        }

        public override void InitializeRecursive(Screen screen)
        {
            criterion = screen.AdvancementTracker.Advancement(AdvancementName).Criteria.TryGetValue(CriterionName, out var val) ? val : null;
            if (criterion == null)
                return;

            Texture = criterion.Icon;
            if (scale == 1)
            {
                var label = GetControlByName("label", true) as UITextBlock;
                label?.SetFont("minecraft", 12);
                label?.SetText(criterion.Name);
            }

            base.InitializeRecursive(screen);
        }

        public override void DrawThis(Display display)
        {
            if (criterion != null)
            {
                if (IsStatic)
                    Tint = Color.White;
                else
                    Tint = criterion.IsCompleted ? Color.White : Color.White * 0.35f;
                display.Draw(Texture, new Rectangle(Left, Top, imageSize, imageSize), Tint);
            } 
        }
    }
}

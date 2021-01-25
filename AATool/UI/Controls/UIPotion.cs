using AATool.DataStructures;
using AATool.Settings;
using AATool.UI.Screens;

namespace AATool.UI.Controls
{
    class UIPotion : UIControl
    {
        public Potion Potion;

        private UIPicture arrow;

        public UIPotion()
        {
            InitializeFromSourceDocument();
        }

        public override void InitializeRecursive(Screen screen)
        {       
            (GetControlByName("potion", true) as UIPicture)?.SetTexture(Potion.Icon);

            arrow = (GetControlByName("arrow", true) as UIPicture);
            arrow?.SetTexture("arrow");

            var label = GetFirstOfType(typeof(UITextBlock)) as UITextBlock;
            label?.SetFont("minecraft", 12);
            label?.SetText(Potion.Name);

            var flow = GetFirstOfType(typeof(UIFlowPanel));
            if (flow == null)
                return;
            for (int i = 0; i < Potion.Ingredients.Count; i++)
            {
                var ingredient = new UIPicture();
                ingredient.FlexWidth = new Size(32, SizeMode.Absolute);
                ingredient.FlexHeight = new Size(32, SizeMode.Absolute);
                ingredient.Margin = new Margin(i * 10 + 20, 0, 0, 0);
                ingredient.SetTexture(Potion.Ingredients[i]);
                flow.AddControl(ingredient);
            }

            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            arrow?.SetTint(MainSettings.Instance.BorderColor);
        }
    }
}

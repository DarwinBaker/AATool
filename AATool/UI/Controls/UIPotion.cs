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
            UIPicture potion = (GetControlByName("potion", true) as UIPicture);
            potion?.SetTexture(Potion.Icon);

            arrow = (GetControlByName("arrow", true) as UIPicture);
            arrow?.SetTexture("arrow");

            var label = GetFirstOfType(typeof(UITextBlock)) as UITextBlock;
            label?.SetText(Potion.Name);

            var flow = GetFirstOfType(typeof(UIFlowPanel));
            if (flow != null)
            {
                for (int i = 0; i < Potion.Ingredients.Count; i++)
                {
                    var ingredient = new UIPicture();
                    ingredient.FlexWidth = new Size(32, SizeMode.Absolute);
                    ingredient.FlexHeight = new Size(32, SizeMode.Absolute);
                    //ingredient.Margin = new Margin(3, 0, 0, 0);
                    ingredient.SetTexture(Potion.Ingredients[i].Icon);
                    flow.AddControl(ingredient);
                }
            }
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            arrow?.SetTint(MainSettings.Instance.BorderColor);
        }
    }
}

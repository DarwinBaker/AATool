using AATool.Data;
using AATool.Settings;
using AATool.UI.Screens;

namespace AATool.UI.Controls
{
    public class UIPotion : UIControl
    {
        public Potion Potion { get; set; }

        private UIPicture arrow;

        public UIPotion()
        {
            this.BuildFromSourceDocument();
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.arrow = this.First<UIPicture>("arrow");
            this.First<UIPicture>("potion").SetTexture(this.Potion.Icon);
            this.First<UITextBlock>().SetText(this.Potion.Name);

            UIFlowPanel flow = this.First<UIFlowPanel>();
            for (int i = 0; i < this.Potion.Ingredients.Count; i++)
            {
                var ingredient = new UIPicture {
                    FlexWidth  = new Size(32, SizeMode.Absolute),
                    FlexHeight = new Size(32, SizeMode.Absolute)
                };
                ingredient.SetTexture(this.Potion.Ingredients[i]);
                flow.AddControl(ingredient);
            }
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time) => 
            this.arrow.SetTint(MainSettings.Instance.BorderColor);
    }
}

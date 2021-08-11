using AATool.UI.Screens;

namespace AATool.UI.Controls
{
    public class UIPlayerPicker : UIButton
    {
        private UIPicture icon;

        public override void InitializeRecursive(UIScreen screen)
        {

            icon.FlexWidth = new Size(32, SizeMode.Absolute);
            icon.FlexHeight = new Size(32, SizeMode.Absolute);

            base.InitializeRecursive(screen);
        }
    }
}

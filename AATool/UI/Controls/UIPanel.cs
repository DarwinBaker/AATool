using AATool.Settings;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIPanel : UIControl
    {
        public Color BackColor;
        public Color BorderColor;

        public override void DrawThis(Display display)
        {
            display.DrawRectangle(Rectangle, MainSettings.Instance.BackColor, MainSettings.Instance.BorderColor, 1);
        }
    }
}

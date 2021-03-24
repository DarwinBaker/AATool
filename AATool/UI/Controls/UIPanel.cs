using AATool.Settings;
using Microsoft.Xna.Framework;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIPanel : UIControl
    {
        public int BorderThickness;
        public Color BackColor;
        public Color BorderColor;

        public UIPanel()
        {
            BorderThickness = 1;
        }

        public override void DrawThis(Display display)
        {
            display.DrawRectangle(Rectangle, MainSettings.Instance.BackColor, MainSettings.Instance.BorderColor, BorderThickness);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            BorderThickness = ParseAttribute(node, "border_thickness", 1);
        }
    }
}

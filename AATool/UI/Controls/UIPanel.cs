using AATool.Graphics;
using AATool.Settings;
using Microsoft.Xna.Framework;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIPanel : UIControl
    {
        public int BorderThickness  { get; set; }
        public Color BackColor      { get; set; }
        public Color BorderColor    { get; set; }

        public UIPanel()
        {
            this.BorderThickness = 1;
        }

        public override void DrawThis(Display display) 
        {
            if (this.SkipDraw)
                return;

            display.DrawRectangle(this.Bounds,
                Config.Main.BackColor, 
                Config.Main.BorderColor, 
                this.BorderThickness, 
                this.Layer);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.BorderThickness = ParseAttribute(node, "border_thickness", 1);
        }
    }
}

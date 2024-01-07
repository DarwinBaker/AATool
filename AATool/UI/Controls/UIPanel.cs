using AATool.Configuration;
using AATool.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIPanel : UIControl
    {
        public Color BackColor      { get; set; }
        public Color BorderColor    { get; set; }
        public int BorderThickness  { get; set; } = 1;
        public bool InnerCorners    { get; set; } = true;

        public UIPanel()
        {
            this.BorderThickness = 1;
        }

        public override void DrawThis(Canvas canvas) 
        {
            if (this.SkipDraw)
                return;

            if (this.InnerCorners)
            {
                canvas.DrawRectangle(this.Bounds,
                    Config.Main.BackColor,
                    Config.Main.BorderColor,
                    this.BorderThickness,
                    this.Layer);
            }
            else
            {
                canvas.DrawRectangle(this.Bounds,
                    Config.Main.BorderColor, 
                    null, 
                    0,
                    this.Layer);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.BorderThickness = Attribute(node, "border_thickness", this.BorderThickness);
            this.InnerCorners = Attribute(node, "inner_corners", this.InnerCorners);
        }
    }
}

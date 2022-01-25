using AATool.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIMinecraftPanel : UIControl
    {
        private const int CORNER_SIZE = 2;

        public string Style { get; private set; }
        public int Scale    { get; private set; }

        private int CornerSize   => CORNER_SIZE * this.Scale;
        private int MiddleWidth  => this.Width - (this.CornerSize * 2);
        private int MiddleHeight => this.Height - (this.CornerSize * 2);

        public UIMinecraftPanel()
        {
            this.Style = "light";
        }

        public override void DrawThis(Canvas canvas)
        {
            string tex = "minecraft_panel_" + this.Style;

            //middle
            canvas.Draw(tex, new Rectangle(this.Left + this.CornerSize, this.Top + this.CornerSize, this.MiddleWidth, this.MiddleHeight),
                new Rectangle(2, 2, 1, 1));

            //top left
            canvas.Draw(tex, new (this.Left, this.Top, this.CornerSize, this.CornerSize),
                new Rectangle(0, 0, 2, 2));
            //top right
            canvas.Draw(tex, new (this.Right - this.CornerSize, this.Top, this.CornerSize, this.CornerSize),
                new Rectangle(3, 0, 2, 2));
            //bottom left
            canvas.Draw(tex, new (this.Left, this.Bottom - this.CornerSize, this.CornerSize, this.CornerSize),
                new Rectangle(0, 3, 2, 2));
            //bottom right
            canvas.Draw(tex, new (this.Right - this.CornerSize, this.Bottom - this.CornerSize, this.CornerSize, this.CornerSize),
                new Rectangle(3, 3, 2, 2));

            //top edge
            canvas.Draw(tex, new (this.Left + this.CornerSize, this.Top, this.MiddleWidth, this.CornerSize),
                new Rectangle(2, 0, 1, 2));
            //bottom edge
            canvas.Draw(tex, new (this.Left + this.CornerSize, this.Bottom - this.CornerSize, this.MiddleWidth, this.CornerSize),
                new Rectangle(2, 3, 1, 2));
            //left edge
            canvas.Draw(tex, new (this.Left, this.Top + this.CornerSize, this.CornerSize, this.MiddleHeight),
                new Rectangle(0, 2, 2, 1));
            //right edge
            canvas.Draw(tex, new (this.Right - this.CornerSize, this.Top + this.CornerSize, this.CornerSize, this.MiddleHeight),
                new Rectangle(3, 2, 2, 1));
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.Style = Attribute(node, "style", "light");
            this.Scale = Attribute(node, "scale", 2);
        }
    }
}

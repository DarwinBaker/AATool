using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIFlowPanel : UIPanel
    {
        public FlowDirection FlowDirection;

        public int CellWidth  = 0;
        public int CellHeight = 0;

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.ReflowChildren();
        }

        public void ReflowChildren()
        {
            if (!this.Children.Any())
                return;

            switch (this.FlowDirection)
            {
                case FlowDirection.LeftToRight:
                    this.ReflowHorizontal(true);
                    break;
                case FlowDirection.RightToLeft:
                    this.ReflowHorizontal(false);
                    break;
                case FlowDirection.TopToBottom:
                    this.ReflowVertical(true);
                    break;
                case FlowDirection.BottomToTop:
                    this.ReflowVertical(false);
                    break;
            }
        }

        private void ReflowHorizontal(bool leftToRight)
        {
            int consumed = 0;
            int remaining = this.Inner.Width;
            int y = this.Inner.Top;
            foreach (UIControl child in this.Children)
            {
                if (child.IsCollapsed)
                    continue;

                int width  = Math.Max(this.CellWidth, child.Width + child.Margin.Horizontal);
                int height = Math.Max(this.CellHeight, child.Height + child.Margin.Vertical);

                if (remaining < width)
                {
                    //start next row
                    consumed  = 0;
                    remaining = this.Inner.Width;
                    y += height;
                }

                //reposition child control
                int x = leftToRight
                    ? this.Inner.Left + consumed
                    : this.Inner.Right - consumed - width;

                child.ResizeRecursive(new Rectangle(x, y, width, height));
                consumed += width;
                remaining -= width;
            }
        }

        private void ReflowVertical(bool topToBottom)
        {
            int consumed = 0;
            int remaining = this.Inner.Height;
            int x = this.Inner.Left;
            foreach (UIControl child in this.Children)
            {
                if (child.IsCollapsed)
                    continue;

                int width  = Math.Max(this.CellWidth, child.Width + child.Margin.Horizontal);
                int height = Math.Max(this.CellHeight, child.Height + child.Margin.Vertical);

                if (remaining < height)
                {
                    //start next row
                    consumed  = 0;
                    remaining = this.Inner.Height;
                    x += width;
                }

                //reposition child control
                int y = topToBottom
                    ? this.Inner.Top + consumed
                    : this.Inner.Bottom - consumed;

                child.ResizeRecursive(new Rectangle(x, y, width, height));
                consumed += height;
                remaining -= height;
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.FlowDirection = Attribute(node, "direction", FlowDirection.LeftToRight);
            this.CellWidth     = Attribute(node, "cell_width", this.CellWidth);
            this.CellHeight    = Attribute(node, "cell_height", this.CellHeight);
        }
    }
}

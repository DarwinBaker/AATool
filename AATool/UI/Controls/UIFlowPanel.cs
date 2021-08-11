using Microsoft.Xna.Framework;
using System;
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
            int currentX = this.FlowDirection is FlowDirection.RightToLeft ? this.Content.Right - this.CellWidth : this.Content.Left;
            int currentY = this.FlowDirection is FlowDirection.BottomToTop ? this.Content.Bottom - this.CellHeight : this.Content.Top;
            int temp = 0;

            foreach (UIControl child in this.Children)
            {
                if (child.IsCollapsed)
                    continue;

                switch (this.FlowDirection)
                {
                    case FlowDirection.LeftToRight:
                        if (currentX + this.CellWidth > this.Content.Right)
                        {
                            //control won't fit horizontally; go to beginning of next row
                            currentX = this.Content.Left;
                            currentY += temp;
                            temp = 0;
                        }
                        break;
                    case FlowDirection.RightToLeft:
                        if (currentX < this.Content.Left)
                        {
                            currentX = this.Content.Right - this.CellWidth;
                            currentY += temp;
                            temp = 0;
                        }
                        break;
                    case FlowDirection.TopToBottom:
                        if (currentY + this.CellHeight > this.Content.Bottom)
                        {
                            //control won't fit vertically; go to top of next column
                            currentY = this.Content.Top;
                            currentX += temp;
                            temp = 0;
                        }
                        break;
                    case FlowDirection.BottomToTop:
                        if (currentY < this.Content.Top)
                        {
                            currentY = this.Content.Bottom - this.CellHeight;
                            currentX += temp;
                            temp = 0;
                        }
                        break;
                }

                //move control into place
                int finalWidth = Math.Max(this.CellWidth, child.Width + child.Margin.Horizontal);
                int finalHeight = Math.Max(this.CellHeight, child.Height + child.Margin.Vertical);
                child.ResizeRecursive(new Rectangle(currentX, currentY, finalWidth, finalHeight));

                //move flow "cursor"
                switch (this.FlowDirection)
                {
                    case FlowDirection.LeftToRight:
                        currentX += Math.Max(child.Width, finalWidth);
                        temp = Math.Max(temp, finalHeight);
                        break;
                    case FlowDirection.RightToLeft:
                        currentX -= Math.Max(child.Width, finalWidth);
                        temp = Math.Max(temp, finalHeight);
                        break;
                    case FlowDirection.TopToBottom:
                        currentY += Math.Max(child.Height, finalHeight);
                        temp = Math.Max(temp, finalWidth);
                        break;
                    case FlowDirection.BottomToTop:
                        currentY -= Math.Max(child.Height, finalHeight);
                        temp = Math.Max(temp, finalWidth);
                        break;
                }
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.FlowDirection = ParseAttribute(node, "direction", FlowDirection.LeftToRight);
            this.CellWidth     = ParseAttribute(node, "cell_width", this.CellWidth);
            this.CellHeight    = ParseAttribute(node, "cell_height", this.CellHeight);
        }
    }
}

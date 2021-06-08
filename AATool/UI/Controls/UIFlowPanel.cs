using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIFlowPanel : UIPanel
    {
        public FlowDirection FlowDirection;

        public int CellWidth = 0;
        public int CellHeight = 0;

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            ReflowChildren();
        }

        public void ReflowChildren()
        {
            int currentX = FlowDirection == FlowDirection.RightToLeft ? ContentRectangle.Right - CellWidth : ContentRectangle.Left;
            int currentY = FlowDirection == FlowDirection.BottomToTop ? ContentRectangle.Bottom - CellHeight : ContentRectangle.Top;
            int temp = 0;

            foreach (var child in Children)
            {
                if (child.IsCollapsed)
                    continue;

                switch (FlowDirection)
                {
                    case FlowDirection.LeftToRight:
                        if (currentX + CellWidth > ContentRectangle.Right)
                        {
                            //control won't fit horizontally; go to beginning of next row
                            currentX = ContentRectangle.Left;
                            currentY += temp;
                            temp = 0;
                        }
                        break;
                    case FlowDirection.RightToLeft:
                        if (currentX < ContentRectangle.Left)
                        {
                            currentX = ContentRectangle.Right - CellWidth;
                            currentY += temp;
                            temp = 0;
                        }
                        break;
                    case FlowDirection.TopToBottom:
                        if (currentY + CellHeight > ContentRectangle.Bottom)
                        {
                            //control won't fit vertically; go to top of next column
                            currentY = ContentRectangle.Top;
                            currentX += temp;
                            temp = 0;
                        }
                        break;
                    case FlowDirection.BottomToTop:
                        if (currentY < ContentRectangle.Top)
                        {
                            currentY = ContentRectangle.Bottom - CellHeight;
                            currentX += temp;
                            temp = 0;
                        }
                        break;
                }

                //move control into place
                child.HorizontalAlign = HorizontalAlign.Left;
                child.VerticalAlign = VerticalAlign.Top;
                int finalWidth = Math.Max(CellWidth, child.Width + child.Margin.Horizontal);
                int finalHeight = Math.Max(CellHeight, child.Height + child.Margin.Vertical);
                child.ResizeRecursive(new Rectangle(currentX, currentY, finalWidth, finalHeight));

                //move flow "cursor"
                switch (FlowDirection)
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
            FlowDirection = ParseAttribute(node, "direction", FlowDirection.LeftToRight);
            CellWidth = ParseAttribute(node, "cell_width", CellWidth);
            CellHeight = ParseAttribute(node, "cell_height", CellHeight);
        }
    }
}

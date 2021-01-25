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
            ReflowChildren();
        }

        protected void ReflowChildren()
        {
            int currentX = ContentRectangle.Left;
            int currentY = ContentRectangle.Top;
            int temp = 0;

            foreach (var child in Children)
            {
                if (FlowDirection == FlowDirection.LeftToRight)
                {
                    if (currentX + CellWidth > ContentRectangle.Right)
                    {
                        //control won't fit horizontally; go to beginning of next row
                        currentX = ContentRectangle.Left;
                        currentY += temp;
                        temp = 0;
                    }
                }
                else
                {
                    //control won't fit vertically; go to top of next column
                    if (currentY + CellHeight > ContentRectangle.Bottom)
                    {
                        currentY = ContentRectangle.Top;
                        currentX += temp;
                        temp = 0;
                    }
                }

                //move control into place
                child.HorizontalAlign = HorizontalAlign.Left;
                child.VerticalAlign = VerticalAlign.Top;
                int finalWidth = Math.Max(CellWidth, child.Width);
                int finalHeight = Math.Max(CellHeight, child.Height);
                child.ResizeRecursive(new Rectangle(currentX, currentY, finalWidth, finalHeight));

                //move flow "cursor"
                if (FlowDirection == FlowDirection.LeftToRight)
                {
                    currentX += child.Width;
                    temp = Math.Max(temp, finalHeight);
                }
                else
                {
                    currentY += child.Height;
                    temp = Math.Max(temp, finalWidth);
                }
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            FlowDirection = ParseAttribute(node, "direction", FlowDirection.LeftToRight);
            CellWidth     = ParseAttribute(node, "cell_width", CellWidth);
            CellHeight    = ParseAttribute(node, "cell_height", CellHeight);
        }
    }
}

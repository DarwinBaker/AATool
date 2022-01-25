using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIFlowCarousel : UICarousel
    {
        protected int CellWidth;
        protected int SplitterWidth;

        private bool isOverflowing;

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);
            this.UpdateSourceList();
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            this.ResizeThis(rectangle);
            this.ReflowChildren();
        } 

        protected override void UpdateThis(Time time)
        {
            int expandedWidth = 0;
            foreach (object item in this.SourceList)
            {
                var control = item as UIControl;
                if (!control.IsCollapsed)
                    expandedWidth += this.CellWidth;
                this.SplitterWidth = this.Width % this.CellWidth;
            }

            if (expandedWidth > this.Width)
            {
                this.Fill();
                this.Slide(time);
            }
        }

        protected override void UpdateSourceList()
        {
            //keep copy of children so we can remove them without dereferencing
            foreach (UIControl child in this.Children)
                this.SourceList.Add(child);
        }

        protected override void Fill()
        {
            if (!this.isOverflowing)
            {
                foreach (UIControl child in this.Children)
                    child.ResizeRecursive(new Rectangle(0, 0, this.Width, this.Height));

                this.Children.Clear();
                this.isOverflowing = true;
            }

            //calculate widths
            int x = this.Children.Count > 0 ? this.Children.Last().Right : 0;
            if (this.Children.Count > 0)
                x = this.RightToLeft ? this.Children.Last().Right : this.Width - this.Children.Last().Left;

            //while more controls will fit, add them
            int attempts = 0;
            while (x < this.Width)
            {
                if (this.NextIndex >= this.SourceList.Count)
                    this.NextIndex = 0;

                if (attempts < this.SourceList.Count)
                {
                    UIControl control = this.NextControl();
                    if (!control.IsCollapsed && !this.Children.Contains(control))
                    {
                        if (this.RightToLeft)
                            control.MoveTo(new Point(x, this.Inner.Top));
                        else
                            control.MoveTo(new Point(this.Width - x - control.Width, this.Inner.Top));
                        x += this.CellWidth;
                        this.AddControl(control);
                    }
                }
                else if (this.SplitterWidth > 0)
                {
                    var splitter = new UIPanel {
                        DrawMode = DrawMode.None
                    };
                    splitter.MoveTo(new Point(x, this.Inner.Top));
                    splitter.ScaleTo(new Point(this.SplitterWidth, this.Inner.Height));
                    x += this.SplitterWidth;
                    this.AddControl(splitter);
                }
                else
                {
                    this.Clear();
                    return;
                }

                this.NextIndex++;
                attempts++;
            }
        }

        protected override UIControl NextControl() => 
            this.SourceList[this.NextIndex] as UIControl;

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.CellWidth = Attribute(node, "cell_width", 0);
        }

        public void ReflowChildren()
        {
            int currentX = this.Inner.Left;
            int currentY = this.Inner.Top;
            foreach (UIControl child in this.Children)
            {
                if (child.IsCollapsed)
                    continue;

                //move control into place
                child.HorizontalAlign = HorizontalAlign.Left;
                child.VerticalAlign   = VerticalAlign.Top;
                int finalWidth        = Math.Max(this.CellWidth, child.Width + child.Margin.Horizontal);
                int finalHeight       = Math.Max(this.Height, child.Height + child.Margin.Vertical);
                child.ResizeRecursive(new Rectangle(currentX, currentY, finalWidth, finalHeight));
                currentX += Math.Max(child.Width, finalWidth);
            }
        }
    }
}

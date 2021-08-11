using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Settings;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public abstract class UICarousel : UIControl
    {
        protected List<object> SourceList;
        protected bool RightToLeft;
        protected double Speed;
        protected int NextIndex;

        private bool isScrolling;
        private double offset;

        protected int Direction => this.RightToLeft ? -1 : 1;

        public UICarousel()
        {
            this.SourceList      = new List<object>();
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign   = VerticalAlign.Top;
            this.isScrolling     = true;
            this.NextIndex       = 0;
        }

        public void Clear()    => this.Children.Clear();
        public void Break()    => this.isScrolling = false;
        public void Continue() => this.isScrolling = true;

        protected abstract UIControl NextControl();
        protected abstract void UpdateSourceList();

        public void SetSpeed(double speed)
        {
            this.Speed = speed;
        }

        public void SetScrollDirection(bool rightToLeft)
        {
            this.RightToLeft = rightToLeft;
            this.Clear();
        }

        protected override void UpdateThis(Time time)
        {
            this.Slide(time);
        }

        public override void MoveTo(Point point)
        {
            if (this.Y != point.Y)
            {
                this.ResizeThis(new Rectangle(point.X, point.Y, this.Width, this.Height));
                this.Clear();
                this.NextIndex = 0;
            }
        }

        protected void Slide(Time time)
        {
            //update speed
            if (this.isScrolling)
                this.offset += this.Speed * time.Delta;

            if (this.offset >= 1)
            {
                //slide controls to the left
                for (int i = 0; i < this.Children.Count; i++)
                    this.Children[i].MoveBy(new Point((int)this.offset * this.Direction, 0));

                //remove controls that leave the viewport
                if (this.Children.Count > 0 && (this.Children[0].Right < 0 || this.Children[0].Left > this.Width))
                    this.RemoveControl(this.Children[0]);
                this.offset -= (int)this.offset;
            }
        }

        protected virtual void Fill()
        {
            //calculate widths
            int x = 0;
            if (this.Children.Count > 0)
                x = this.RightToLeft ? this.Children.Last().Right : this.Width - this.Children.Last().Left;

            //while more controls will fit, add them
            while (x < this.Width)
            {
                if (this.SourceList.Count == 0)
                    return;

                if (this.NextIndex >= this.SourceList.Count)
                    this.NextIndex = 0;

                var control = this.NextControl();
                control.ResizeRecursive(this.Bounds);
                control.VerticalAlign = VerticalAlign.Top;
                if (this.RightToLeft)
                    control.MoveTo(new Point(x, this.Content.Top));
                else
                    control.MoveTo(new Point(this.Width - x - control.Width, this.Content.Top));

                this.AddControl(control);

                //add control width to total width
                x += control.Width;
                this.NextIndex++;
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            RightToLeft = ParseAttribute(node, "right_to_left", true);
            Speed = ParseAttribute(node, "speed", 1);
        }
    }
}

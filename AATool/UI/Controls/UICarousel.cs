using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Settings;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public abstract class UICarousel : UIControl
    {
        protected List<object> SourceList;
        protected int NextIndex;
        
        private Timer timer;
        private int moveRatio = 1;

        protected override void UpdateThis(Time time) => Slide(time);
        protected void Clear() => Children.Clear();
        protected abstract UIControl NextControl();
        protected abstract void UpdateSourceList();

        public UICarousel()
        {
            SourceList = new List<object>();
            NextIndex = 0;
            timer = new Timer(1 / 60.0);
            HorizontalAlign = HorizontalAlign.Left;
            VerticalAlign = VerticalAlign.Top;
        }

        public override void MoveTo(Point point)
        {
            if (Y != point.Y)
            {
                ResizeThis(new Rectangle(point.X, point.Y, Width, Height));
                Clear();
            }
        }

        protected void Slide(Time time)
        {
            //update speed
            double targetInterval = Math.Abs(OverlaySettings.Instance.Speed - 4) * (1.0 / 60);
            if (timer.Duration != targetInterval)
                timer = new Timer(targetInterval);

            timer.Update(time);
            if (timer.IsExpired)
            {
                //slide controls to the left
                for (int i = 0; i < Children.Count; i++)
                    Children[i].MoveBy(new Point(-moveRatio, 0));

                //remove controls that leave the viewport
                if (Children.Count > 0 && Children[0].Right < 0)
                    RemoveControl(Children[0]);
                timer.Reset();
            }
        }

        protected virtual void Fill()
        {
            //calculate widths
            int gap = 20;
            int singleWidth = 102 + gap;
            int totalWidth = Children.Count > 0 ? Children.Last().Right : 0;

            //while more controls will fit, add them
            while (totalWidth < Width + singleWidth)
            {
                if (SourceList.Count == 0)
                    return;

                if (NextIndex >= SourceList.Count)
                    NextIndex = 0;

                var control = NextControl();
                control.ResizeRecursive(Rectangle);
                control.VerticalAlign = VerticalAlign.Top;
                control.MoveTo(new Point(totalWidth, ContentRectangle.Top));
                AddControl(control);

                //add control width to total width
                totalWidth += control.Width;
                NextIndex++;
            }
        }
    }
}

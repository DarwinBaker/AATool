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

        protected int Direction => OverlaySettings.Instance.RightToLeft ? 1 : -1;
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

        protected override void UpdateThis(Time time)
        {
            //reset carousel if settings change
            if (OverlaySettings.Instance.ValueChanged(OverlaySettings.RIGHT_TO_LEFT))
                Clear();
            if (OverlaySettings.Instance.ValueChanged(OverlaySettings.ONLY_FAVORITES))
                Clear();

            Slide(time);
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
                    Children[i].MoveBy(new Point(-Direction, 0));

                //remove controls that leave the viewport
                if (Children.Count > 0 && (Children[0].Right < 0 || Children[0].Left > Width))
                    RemoveControl(Children[0]);
                timer.Reset();
            }
        }

        protected virtual void Fill()
        {
            //calculate widths
            int x = 0;
            if (Children.Count > 0)
                x = OverlaySettings.Instance.RightToLeft ? Children.Last().Right : Width - Children.Last().Left;

            //while more controls will fit, add them
            while (x < Width)
            {
                if (SourceList.Count == 0)
                    return;

                if (NextIndex >= SourceList.Count)
                    NextIndex = 0;

                var control = NextControl();
                control.ResizeRecursive(Rectangle);
                control.VerticalAlign = VerticalAlign.Top;
                if (OverlaySettings.Instance.RightToLeft)
                    control.MoveTo(new Point(x, ContentRectangle.Top));
                else
                    control.MoveTo(new Point(Width - x - control.Width, ContentRectangle.Top));
                
                AddControl(control);

                //add control width to total width
                x += control.Width;
                NextIndex++;
            }
        }
    }
}

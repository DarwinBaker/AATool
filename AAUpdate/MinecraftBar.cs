using System;
using System.Drawing;
using System.Windows.Forms;

namespace AAUpdate
{
    public partial class MinecraftBar : UserControl
    {
        private const int SEGMENT_WIDTH  = 20;
        private const int SEGMENT_HEIGHT = 10;
        private const float SMOOTH_RATE  = 5f;

        public int Value { get; private set; } = 0;
        public int Min   { get; private set; } = 0;
        public int Max   { get; private set; } = 100;

        private Timer animationTimer;
        private float displayValue = 0;

        public MinecraftBar()
        {
            animationTimer = new Timer();
            animationTimer.Interval = 16;
            animationTimer.Tick += OnTick;
            animationTimer.Start();
        }

        public void SetValue(int value)
        {
            if (Value == value)
                return;
            Value = Math.Min(Math.Max(value, Min), Max);
            if (displayValue > value)
                displayValue = 0;
            Invalidate();
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (displayValue < Value)
            {
                displayValue = Math.Min(displayValue + SMOOTH_RATE, Value);
                Invalidate();
            }
            else if (displayValue > Value)
            {
                displayValue = 0;
                Invalidate();
            }
        }

        protected override CreateParams CreateParams
        {
            //prevents progress bars from flickering
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x2000000;
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //draw background segments
            var graphics = e.Graphics;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphics.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.None;
            graphics.PixelOffsetMode   = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            for (int i = 0; i < Width / SEGMENT_WIDTH; i++)
            {
                var segmentRect = new Rectangle(SEGMENT_WIDTH * i, 0, SEGMENT_WIDTH, SEGMENT_HEIGHT);
                if (i == 0)
                    graphics.DrawImage(Properties.Resources.bar_inactive_left, segmentRect);
                else if (i == (Width / SEGMENT_WIDTH) - 1)
                    graphics.DrawImage(Properties.Resources.bar_inactive_right, segmentRect);
                else
                    graphics.DrawImage(Properties.Resources.bar_inactive_middle, segmentRect);

            }
            graphics.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //draw foreground segments
            var graphics = e.Graphics;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            for (int i = 0; i < (int)Math.Round(Width * (displayValue - Min) / (Max - Min)) / SEGMENT_WIDTH; i++)
            {
                var segmentRect = new Rectangle(SEGMENT_WIDTH * i, 0, SEGMENT_WIDTH, SEGMENT_HEIGHT);
                if (i == 0)
                    graphics.DrawImage(Properties.Resources.bar_active_left, segmentRect);
                else if (i == (Width / SEGMENT_WIDTH) - 1)
                    graphics.DrawImage(Properties.Resources.bar_active_right, segmentRect);
                else
                    graphics.DrawImage(Properties.Resources.bar_active_middle, segmentRect);
            }
            graphics.Dispose();
        }
    }
}

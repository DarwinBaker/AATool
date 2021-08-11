using Microsoft.Xna.Framework;

namespace AATool.UI
{
    public class Margin
    {
        private Size left;
        private Size right;
        private Size top;
        private Size bottom;

        public int Top        => top.Absolute;
        public int Bottom     => bottom.Absolute;
        public int Left       => left.Absolute;
        public int Right      => right.Absolute;
        public int Horizontal => Left + Right;
        public int Vertical   => Top + Bottom;

        public void Resize(Point maxSize)
        {
            this.top.Resize(maxSize.X);
            this.bottom.Resize(maxSize.X);
            this.left.Resize(maxSize.Y);
            this.right.Resize(maxSize.Y);
        }

        public Margin(Size left, Size right, Size top, Size bottom)
        {
            this.left   = left   ?? new Size();
            this.right  = right  ?? new Size();
            this.top    = top    ?? new Size();
            this.bottom = bottom ?? new Size();
        }

        public Margin(int left, int right, int top, int bottom)
        {
            this.left   = new Size(left);
            this.right  = new Size(right);
            this.top    = new Size(top);
            this.bottom = new Size(bottom);
            this.Resize(Point.Zero);
        }

        public Margin(Size size) 
            : this(size, size, size, size) { }

        public Margin(float size, SizeMode mode) 
            : this(new Size(size, mode)) { }

        public static Margin Parse(string encoded)
        {
            string[] csv = encoded.Split(',');
            return csv.Length == 4
                ? new Margin(Size.Parse(csv[0]), Size.Parse(csv[1]), Size.Parse(csv[2]), Size.Parse(csv[3]))
                : new Margin(Size.Parse(csv[0]));
        }
    }
}

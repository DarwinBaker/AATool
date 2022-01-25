using Microsoft.Xna.Framework;

namespace AATool.UI
{
    public class Margin
    {
        private readonly Size left;
        private readonly Size right;
        private readonly Size top;
        private readonly Size bottom;

        public int Top        => this.top;
        public int Bottom     => this.bottom;
        public int Left       => this.left;
        public int Right      => this.right;
        public int Horizontal => this.Left + this.Right;
        public int Vertical   => this.Top + this.Bottom;

        public Margin(int left, int right, int top, int bottom)
        {
            this.left   = new Size(left);
            this.right  = new Size(right);
            this.top    = new Size(top);
            this.bottom = new Size(bottom);
            this.Resize(Point.Zero);
        }

        public Margin(Size left, Size right, Size top, Size bottom)
        {
            this.left   = left   ?? Size.Zero;
            this.right  = right  ?? Size.Zero;
            this.top    = top    ?? Size.Zero;
            this.bottom = bottom ?? Size.Zero;
        }

        public Margin(Size size) 
            : this(size, size, size, size) { }

        public Margin(float size, SizeMode mode) 
            : this(new Size(size, mode)) { }

        public void Resize(Point maxSize)
        {
            this.top.Resize(maxSize.X);
            this.bottom.Resize(maxSize.X);
            this.left.Resize(maxSize.Y);
            this.right.Resize(maxSize.Y);
        }

        public static Margin Parse(string encoded)
        {
            string[] tokens = encoded.Split(',');
            return tokens.Length is 4
                ? new Margin(Size.Parse(tokens[0]), Size.Parse(tokens[1]), Size.Parse(tokens[2]), Size.Parse(tokens[3]))
                : new Margin(Size.Parse(tokens[0]));
        }
    }
}

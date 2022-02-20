using Microsoft.Xna.Framework;

namespace AATool.Graphics
{
    public class Sprite
    {
        public const string ResolutionFlag = "^";
        public const string FramesFlag = "$";
        public const char ColumnsDelimiter = 'x';
        public const string PaddingFlag = "~";
        public const int MaxAnimationColumns = 32;

        public Rectangle Source { get; protected set; }

        public readonly Vector2 Origin;

        public Point Offset => this.Source.Location;
        public int X => this.Source.X;
        public int Y => this.Source.Y;
        public int Width  => this.Source.Width;
        public int Height => this.Source.Height;

        public Sprite(Rectangle source)
        {
            this.Source = source;
            this.Origin = new Vector2(source.Width / 2, source.Height / 2);
        }
    }
}

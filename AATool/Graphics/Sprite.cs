using Microsoft.Xna.Framework;

namespace AATool.Graphics
{
    public readonly struct Sprite
    {
        public readonly Rectangle Source;
        public readonly int Frames;

        public int X      => this.Source.X;
        public int Y      => this.Source.Y;
        public int Width  => this.Source.Width;
        public int Height => this.Source.Height;

        public Sprite(Rectangle source, int frames)
        {
            this.Source = source;
            this.Frames = frames;
        }
    }
}

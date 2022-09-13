using System;
using Microsoft.Xna.Framework;

namespace AATool.Graphics
{
    public class AnimatedSprite : Sprite
    {
        public readonly int Frames;

        private readonly Rectangle fullBounds;
        private readonly decimal speed;
        private readonly int columns;
        private readonly int singleWidth;
        private readonly int singleHeight;

        public int CurrentFrame { get; private set; } = -1;

        public AnimatedSprite(Rectangle source, int frames, int columns, decimal speed) : base(source)
        {
            this.fullBounds = source;
            this.Frames = frames;
            this.columns = Math.Max(columns, 1);
            this.speed = Math.Max(speed, 0.1m);
            this.singleWidth = this.columns > 0 ? this.Width / this.columns : source.Width;
            this.singleHeight = this.Height / (int)Math.Ceiling(this.Frames / (double)this.columns);
        }

        public void Animate(decimal animationTime)
        {
            decimal scaledTime = animationTime / this.speed;
            decimal loops = Math.Floor(scaledTime / this.Frames);
            int wrapped = (int)(scaledTime - (loops * this.Frames));
            if (this.CurrentFrame != wrapped)
            {
                //convert wrapped frame index to x,y offset
                this.CurrentFrame = wrapped;
                int x = this.CurrentFrame % this.columns * this.singleWidth;
                int y = this.CurrentFrame / this.columns * this.singleHeight;

                //update current source rectangle
                this.Source = new Rectangle(
                    this.fullBounds.X + x,
                    this.fullBounds.Y + y,
                    this.singleWidth,
                    this.singleHeight);
            }
        }
    }
}

using Microsoft.Xna.Framework;

namespace AATool.Graphics
{
    public struct Sprite
    {
        public Rectangle SourceRectangle;
        public int FrameCount;

        public Sprite(Rectangle sourceRectangle, int frameCount)
        {
            SourceRectangle = sourceRectangle;
            FrameCount = frameCount;
        }
    }
}

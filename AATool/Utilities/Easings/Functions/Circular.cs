using System;

namespace AATool.Utilities.Easings.Functions
{
    public class Circular : IEasingFunction
    {
        public float In(float i)
        {
            return 1f - (float)Math.Sqrt(1f - i * i);
        }

        public float Out(float i)
        {
            return (float)Math.Sqrt(1f - ((i -= 1f) * i));
        }

        public float InOut(float i)
        {
            if ((i *= 2f) < 1f) return -0.5f * (float)(Math.Sqrt(1f - i * i) - 1);
            return 0.5f * (float)(Math.Sqrt(1f - (i -= 2f) * i) + 1f);
        }
    }
}

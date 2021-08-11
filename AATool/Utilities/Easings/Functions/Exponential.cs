using System;

namespace AATool.Utilities.Easings.Functions
{
    public class Exponential : IEasingFunction
    {
        public float In(float i)
        {
            return i is 0f ? 0f : (float)Math.Pow(1024f, i - 1f);
        }

        public float Out(float i)
        {
            return i is 1f ? 1f : 1f - (float)Math.Pow(2f, -10f * i);
        }

        public float InOut(float i)
        {
            if (i is 0f) return 0f;
            if (i is 1f) return 1f;
            if ((i *= 2f) < 1f) return 0.5f * (float)Math.Pow(1024f, i - 1f);
            return 0.5f * (float)(-Math.Pow(2f, -10f * (i - 1f)) + 2f);
        }
    }
}

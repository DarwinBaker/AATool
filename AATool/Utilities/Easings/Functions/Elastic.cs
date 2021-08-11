using System;

namespace AATool.Utilities.Easings.Functions
{
    public class Elastic : IEasingFunction
    {
        public float In(float i)
        {
            if (i is 0) return 0;
            if (i is 1) return 1;
            return -(float)(Math.Pow(2f, 10f * (i -= 1f)) * Math.Sin((i - 0.1f) * (2f * Math.PI) / 0.4f));
        }

        public float Out(float i)
        {
            if (i is 0) return 0;
            if (i is 1) return 1;
            return (float)(Math.Pow(2f, -10f * i) * Math.Sin((i - 0.1f) * (2f * Math.PI) / 0.4f) + 1f);
        }

        public float InOut(float i)
        {
            if ((i *= 2f) < 1f) return -(float)(0.5f * Math.Pow(2f, 10f * (i -= 1f)) * Math.Sin((i - 0.1f) * (2f * Math.PI) / 0.4f));
            return (float)(Math.Pow(2f, -10f * (i -= 1f)) * Math.Sin((i - 0.1f) * (2f * Math.PI) / 0.4f) * 0.5f + 1f);
        }
    }
}

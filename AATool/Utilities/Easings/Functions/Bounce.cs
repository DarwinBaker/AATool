

namespace AATool.Utilities.Easings.Functions
{
    public class Bounce : IEasingFunction
    {
        public float In(float i)
        {
            return 1f - Out(1f - i);
        }

        public float Out(float i)
        {
            if (i < (1f / 2.75f))
                return 7.5625f * i * i;
            else if (i < (2f / 2.75f))
                return 7.5625f * (i -= (1.5f / 2.75f)) * i + 0.75f;
            else if (i < (2.5f / 2.75f))
                return 7.5625f * (i -= (2.25f / 2.75f)) * i + 0.9375f;
            else
                return 7.5625f * (i -= (2.625f / 2.75f)) * i + 0.984375f;
        }

        public float InOut(float i)
        {
            if (i < 0.5f) return In(i * 2f) * 0.5f;
            return Out(i * 2f - 1f) * 0.5f + 0.5f;
        }
    }
}



namespace AATool.Utilities.Easings.Functions
{
    public class Quartic : IEasingFunction
    {
        public float In(float i)
        {
            return i * i * i * i;
        }

        public float Out(float i)
        {
            return 1f - ((i -= 1f) * i * i * i);
        }

        public float InOut(float i)
        {
            if ((i *= 2f) < 1f) return 0.5f * i * i * i * i;
            return -0.5f * ((i -= 2f) * i * i * i - 2f);
        }
    }
}

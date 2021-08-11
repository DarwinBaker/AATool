
namespace AATool.Utilities.Easings.Functions
{
    public class Quadratic : IEasingFunction
    {
        public float In(float i)
        {
            return i * i;
        }

        public float Out(float i)
        {
            return i * (2f - i);
        }

        public float InOut(float i)
        {
            if ((i *= 2f) < 1f) return 0.5f * i * i;
            return -0.5f * ((i -= 1f) * (i - 2f) - 1f);
        }

        public float Bezier(float i, float c)
        {
            return c * 2 * i * (1 - i) + i * i;
        }
    }
}

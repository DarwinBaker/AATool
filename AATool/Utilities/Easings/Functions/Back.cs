

namespace AATool.Utilities.Easings.Functions
{
    public class Back : IEasingFunction
    {
        private const float S1 = 1.70158f;
        private const float S2 = 2.5949095f;

        public float In(float i) => i * i * ((S1 + 1f) * i - S1);

        public float Out(float i)
        {
            return ((i -= 1f) * i * (((S1 + 1f) * i) + S1)) + 1f;
        }

        public float InOut(float i)
        {
            if ((i *= 2f) < 1f) return 0.5f * (i * i * (((S2 + 1f) * i) - S2));
            return 0.5f * (((i -= 2f) * i * (((S2 + 1f) * i) + S2)) + 2f);
        }
    }
}

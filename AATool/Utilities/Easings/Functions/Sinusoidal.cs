using System;

namespace AATool.Utilities.Easings.Functions
{
    public class Sinusoidal : IEasingFunction
    {
        public float In(float i)
        {
            return 1f - (float)Math.Cos(i * Math.PI / 2f);
        }

        public float Out(float i)
        {
            return (float)Math.Sin(i * Math.PI / 2f);
        }

        public float InOut(float i)
        {
            return 0.5f * (1f - (float)Math.Cos(Math.PI * i));
        }
    };
}

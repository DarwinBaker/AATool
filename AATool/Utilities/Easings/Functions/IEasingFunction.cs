

namespace AATool.Utilities.Easings
{
    public interface IEasingFunction
    {
        public abstract float In(float i);
        public abstract float Out(float i);
        public abstract float InOut(float i);
    }
}

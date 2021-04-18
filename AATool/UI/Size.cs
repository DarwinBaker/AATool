
using System.Globalization;

namespace AATool.UI
{
    public class Size
    {
        public const char CHAR_RELATIVE = '*';

        public double InternalValue { get; private set; }
        public int Absolute         { get; private set; }
        public SizeMode Mode        { get; private set; }

        public int GetAbsoluteInt(int maxValue)          => (int)GetAbsoluteDouble(maxValue);
        public double GetAbsoluteDouble(double maxValue) => Mode == SizeMode.Absolute ? InternalValue : InternalValue * maxValue;

        public Size() { InternalValue = 0; }
        public Size(double value, SizeMode sizeMode)
        {
            InternalValue = value;
            Mode = sizeMode;
        }

        public void Resize(int maxValue)
        {
            if (Mode == SizeMode.Absolute)
                Absolute = (int)InternalValue;
            else
                Absolute = (int)(InternalValue * maxValue);
        }

        public static Size Parse(string encoded)
        {
            if (encoded[encoded.Length - 1] == CHAR_RELATIVE)
                return new Size(double.Parse(encoded.Substring(0, encoded.Length - 1), NumberStyles.Any, CultureInfo.InvariantCulture), SizeMode.Relative);
            return new Size(double.Parse(encoded, NumberStyles.Any, CultureInfo.InvariantCulture), SizeMode.Absolute);
        }

        public static Size operator *(Size size, double multiplier)
        {
            return new Size(size.InternalValue * multiplier, size.Mode);
        }
    }
}

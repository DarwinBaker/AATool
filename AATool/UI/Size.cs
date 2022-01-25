
using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AATool.UI
{
    public class Size
    {
        public static Size operator *(Size size, double multiplier) => new(size.InternalValue * multiplier, size.Mode);

        public static implicit operator int(Size size) => size.Absolute;

        public const char RelativeSuffix = '*';
        public static readonly Size Zero = new (0);

        public double InternalValue { get; private set; }
        public int Absolute { get; private set; }
        public SizeMode Mode { get; private set; }

        public int GetAbsoluteInt(int maxValue = 0) => (int)this.GetAbsoluteDouble(maxValue);

        public double GetAbsoluteDouble(double range = 0)
        {
            return this.Mode is SizeMode.Absolute
                ? this.InternalValue
                : this.InternalValue * range;
        }

        public Size() { }

        public Size(int value)
        {
            this.InternalValue = value;
            this.Absolute = (int)this.InternalValue;
            this.Mode = SizeMode.Absolute;
        }

        public Size(double value, SizeMode sizeMode = SizeMode.Absolute)
        {
            this.InternalValue = value;
            this.Mode = sizeMode;
            if (this.Mode is SizeMode.Absolute)
                this.Absolute = (int)this.InternalValue;
        }

        public void Resize(int range)
        {
            this.Absolute = this.Mode is SizeMode.Absolute
                ? (int)this.InternalValue
                : (int)(this.InternalValue * range);
        }

        public static Size Parse(string code)
        {
            if (string.IsNullOrEmpty(code))
                return Zero;

            string[] tokens = code.Split(' ');
            return tokens.Length is 1
                ? ParseSimple(code, tokens)
                : ParseExpression(tokens);
        }

        private static Size ParseSimple(string code, string[] tokens)
        {
            if (code.Last() is RelativeSuffix)
            {
                //handle relative size
                string number = code.Substring(0, code.Length - 1);
                return double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out double value)
                    ? new Size(value, SizeMode.Relative)
                    : Zero;
            }
            else
            {
                //simple number
                return double.TryParse(tokens[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double value)
                    ? new Size(value)
                    : Zero;
            }
        }

        private static Size ParseExpression(string[] tokens)
        {
            //handle expressions
            if (!double.TryParse(tokens[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
                return Zero;
            if (!double.TryParse(tokens[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                return Zero;

            return tokens[1] switch {
                "+" => new Size(x + y),
                "-" => new Size(x - y),
                "*" => new Size(x * y),
                "/" => new Size(x / y),
                "%" => new Size(x % y),
                "^" => new Size(Math.Pow(x, y)),
                _ => Zero
            };
        }
    }
}


using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AATool.UI
{
    public class Size
    {
        public static Size Parse(string code)
{
            string[] tokens = code.Split(' ');
            if (tokens.Length is 1)
            {
                if (code.Last() is CHAR_RELATIVE)
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
            else
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
                    "^" => new Size(Math.Pow(x, y))
                };
            }
        }

        public static Size operator *(Size size, double multiplier) => new(size.InternalValue * multiplier, size.Mode);

        public const char CHAR_RELATIVE = '*';

        public static readonly Size Zero = new (0);

        public double InternalValue { get; private set; }
        public int Absolute         { get; private set; }
        public SizeMode Mode        { get; private set; }

        public int GetAbsoluteInt(int maxValue) => (int)this.GetAbsoluteDouble(maxValue);
        public double GetAbsoluteDouble(double maxValue)
        { 
            return this.Mode == SizeMode.Absolute
                ? this.InternalValue
                : this.InternalValue * maxValue;
        }

        public Size() 
        { 
            this.InternalValue = 0; 
        }

        public Size(double value, SizeMode sizeMode = SizeMode.Absolute)
        {
            this.InternalValue = value;
            this.Mode          = sizeMode;
        }

        public void Resize(int maxValue)
        {
            this.Absolute = this.Mode is SizeMode.Absolute 
                ? (int) this.InternalValue 
                : (int) (this.InternalValue * maxValue);
        }
    }
}


namespace AATool.Net
{
    public static class AAKey
    {
        public const string RandomCharacters = "0123456789";
        public const string Prefix = "AAKEY-";
        public const int Length = 16;

        public static string Strip(string fullKey) 
            => fullKey?.Replace(Prefix, "").Replace("-", "");
    }
}

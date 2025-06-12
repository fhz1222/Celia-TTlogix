using System.Linq;

namespace TT.Services.Utilities
{
    public static class StringExtensions
    {
        public static string ForceLength(this string v, int length) => v.PadRight(length).Substring(0, length);

        public static string Concat(this string a, params string[] bs) => bs.Aggregate(a, (x, y) => x + y);
    }
}

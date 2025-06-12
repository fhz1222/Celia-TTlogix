using System.Linq;

namespace TT.Common.Extensions
{
    public static class GenericExtensions
    {
        public static bool In<T>(this T value, params T[] values)
            => values.Any(v => v.Equals(value));

        public static bool NotIn<T>(this T value, params T[] values)
            => !In(value, values);
    }
}

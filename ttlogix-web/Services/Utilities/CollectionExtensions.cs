using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TT.Services.Services.Utilities
{
    public static class CollectionExtensions
    {
        public static Stream ToMemoryStream(this IEnumerable<string> data)
        {
            var stream = new MemoryStream();
            var outputFile = new StreamWriter(stream);

            foreach (var line in data)
            {
                outputFile.WriteLine(line);
            }

            outputFile.Flush();
            stream.Position = 0;
            return stream;
        }

        public static bool None<T>(this IEnumerable<T> collection)
            => !collection.Any();

        public static bool None<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
            => !collection.Any(predicate);

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> items)
            => items.Select((item, index) => (item, index));
    }
}

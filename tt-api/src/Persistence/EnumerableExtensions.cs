namespace Persistence;

internal static class EnumerableExtensions
{
    /// <summary>
    /// Max WHERE IN collection count is 2100, and this helps with batching the requests.
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    internal static List<string[]> ChunkCollectionForSqlServer(this string[] collection)
    {
        var maxCollectionSize = 2000;
        return collection.Distinct().Chunk(maxCollectionSize).ToList();
    }
}

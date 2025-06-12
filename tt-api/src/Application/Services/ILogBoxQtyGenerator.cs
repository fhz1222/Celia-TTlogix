namespace Application.Services;

public static class ILogBoxQtyGenerator
{
    public static IEnumerable<int> Generate(int total, int boxQty)
    {
        var remainder = total % boxQty;
        if (remainder == 0)
        {
            return Enumerable.Repeat(boxQty, total / boxQty);
        }
        else
        {
            return Enumerable.Repeat(boxQty, total / boxQty).Concat(Enumerable.Repeat(remainder, 1));
        }
    }
}

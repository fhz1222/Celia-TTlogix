using Domain.ValueObjects;

namespace Application.UseCases.StockTake.Queries.GetStockTakeStandByLocations;

public class StockTakeLocationDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Whscode { get; set; } = null!;
}

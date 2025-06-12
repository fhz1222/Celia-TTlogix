using Domain.ValueObjects;

namespace Application.UseCases.StockTake.Commands.UpdateStockTake;

public class UpdateStockTakeDto
{
    public string JobNo { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string Remark { get; set; } = null!;
}

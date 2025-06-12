using Application.Common.Models;

namespace Application.UseCases.StockTransferReversals.Queries.GetReversibleStockTransfers;

public class GetReversibleStockTransfersDtoFilter
{
    public PaginationQuery Pagination { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string? StfJobNo { get; set; } = null!;
    public DateTime? NewerThan { get; set; }
}

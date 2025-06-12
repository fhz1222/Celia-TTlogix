
namespace Application.UseCases.Storage.Queries.GetPalletsForILogStockDiscrepancyReport;

public class StockDiscrepancyReportPalletDtoFilter
{
    public string[] WHSCodes { get; set; } = Array.Empty<string>();
}

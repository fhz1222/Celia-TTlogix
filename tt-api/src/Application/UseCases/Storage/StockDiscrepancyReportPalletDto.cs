
namespace Application.UseCases.StorageDetails;

public class StockDiscrepancyReportPalletDto
{
    public string PID { get; set; } = null!;
    public decimal Qty { get; set; }
    public int Ownership { get; set; }
    public string LocationCode { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
}
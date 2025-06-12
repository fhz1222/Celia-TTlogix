namespace Domain.Entities;

public class StockTransferReversalDetail
{
    public string JobNo { get; set; } = null!;
    public string Pid { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string OriginalSupplierId { get; set; } = null!;
    public string OriginalLocationCode { get; set; } = null!;
    public string LocationCode { get; set; } = null!;
    public string OriginalWhscode { get; set; } = null!;
    public string Whscode { get; set; } = null!;
    public string TransferredBy { get; set; } = null!;
    public DateTime? TransferredDate { get; set; }
}
namespace Persistence.Entities;

public class TtPriceMaster
{
    public string CustomerCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string ProductCode1 { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public decimal BuyingPrice { get; set; }
    public string LastUpdatedInbound { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime? CreatedDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisedDate { get; set; }
    public decimal SellingPrice { get; set; }
    public string LastUpdatedOutbound { get; set; } = null!;
    public string? OutRevisedBy { get; set; }
    public DateTime? OutRevisedDate { get; set; }
}

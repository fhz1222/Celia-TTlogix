
namespace Application.UseCases.StorageDetails;

public class ILogInboundPalletWithTypeDto
{
    public string LocationCode { get; set; } = null!;
    public string PID { get; set; } = null!;
    public string PalletType { get; set; } = null!;
    public bool IsCPart { get; set; }
    public decimal Height { get; set; }
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    public string ProductCode { get; set; } = null!;
    public DateTime InboundDate { get; set; }
    public int Ownership { get; set; }
    public DateTime PutawayDate { get; set; }
    public bool IsQuarantine { get; set; }
    public string CustomerCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public decimal Qty { get; set; }
    public string? ExternalPID { get; set; }
}
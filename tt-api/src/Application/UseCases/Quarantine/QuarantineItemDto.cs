namespace Application.UseCases.Quarantine;

public class QuarantineItemDto
{
    public string PID { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public int Qty { get; set; }
    public string Location { get; set; } = null!;
    public int DecimalNum { get; set; }
    public string? Reason { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? QuarantineDate { get; set; }
}

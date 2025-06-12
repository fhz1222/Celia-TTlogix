namespace Application.UseCases.Quarantine.Queries.GetQuarantineItems;

public class QuarantineItemDtoFilter
{
    public string WhsCode { get; set; } = null!;
    public string? CustomerCode { get; set; } = null!;
    public string? SupplierId { get; set; }
    public string? ProductCode { get; set; }
    public string? Location { get; set; }
    public string? Reason { get; set; }
    public string? PID { get; set; }
    public string? CreatedBy { get; set; }
    public DtoFilterDateTimeRange QuarantineDate { get; set; } = new DtoFilterDateTimeRange();
    public DtoFilterIntRange Qty { get; set; } = new DtoFilterIntRange();
    public DtoFilterIntRange DecimalNum { get; set; } = new DtoFilterIntRange();
}
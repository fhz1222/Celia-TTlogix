using Domain.ValueObjects;

namespace Application.UseCases.RelocationLogs;

public class RelocationLogDto
{
    public string PID { get; set; } = null!;
    public string? ExternalPID { get; set; }
    public string ProductCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public int Qty { get; set; }
    public string OldWhsCode { get; set; } = null!;
    public string OldLocation { get; set; } = null!;
    public string NewWhsCode { get; set; } = null!;
    public string NewLocation { get; set; } = null!;
    public ScannerType ScannerType { get; set; } = null!;
    public string RelocatedBy { get; set; }
    public DateTime RelocationDate { get; set; }
    public string CustomerCode { get; set; }
    public string CustomerName { get; set; }
}

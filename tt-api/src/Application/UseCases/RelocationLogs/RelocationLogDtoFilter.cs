using Domain.ValueObjects;

namespace Application.UseCases.RelocationLogs.Queries.GetRelocationLogs;

public class RelocationLogDtoFilter
{
    public string? PID { get; set; }
    public string? ExternalPID { get; set; }
    public string? ProductCode { get; set; }
    public string? SupplierId { get; set; }
    public DtoFilterIntRange Qty { get; set; } = null!;
    public string? OldWhsCode { get; set; }
    public string? OldLocation { get; set; }
    public string? NewWhsCode { get; set; }
    public string? NewLocation { get; set; }
    public ScannerType? ScannerType { get; set; }
    public string? RelocatedBy { get; set; }
    public DtoFilterDateTimeRange RelocationDate { get; set; } = new DtoFilterDateTimeRange();
    public string? CustomerCode { get; set; }

    // TODO why not a command dto validator?
    public bool MandatoryConditionsAreSet => RelocationDate.To.HasValue && RelocationDate.From.HasValue || PID != null || ExternalPID != null;

    // TODO why not a command dto validator?
    public bool DatesAreValid
    {
        get
        {
            if (RelocationDate.To.HasValue && RelocationDate.From.HasValue)
            {
                DateTime from = RelocationDate.From.Value.Date;
                DateTime to = RelocationDate.To.Value.Date;
                return from < to && from.AddMonths(3) >= to;
            }
            return true;
        }
    }
}
using Domain.ValueObjects;

namespace Domain.Entities;

public class RelocationLog
{
    public string PalletId { get; set; } = null!;
    public string ExternalPalletId { get; set; } = null!;
    public Location SourceLocation { get; set; } = null!;
    public Location TargetLocation { get; set; } = null!;
    public ScannerType ScannerType { get; set; }
    public string RelocatedBy { get; set; }
    public DateTime RelocatedOn { get; set; }
}

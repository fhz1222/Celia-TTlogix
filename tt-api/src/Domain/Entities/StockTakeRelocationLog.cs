using Domain.ValueObjects;

namespace Domain.Entities;

public class StockTakeRelocationLog
{
    public string JobNo { get; set; } = null!;
    public string Pid { get; set; } = null!;
    public StockTakeRelocationLogType TransType { get; set; } = null!;
    public string OldLocationCode { get; set; } = null!;
    public string OldWhscode { get; set; } = null!;
    public string NewLocationCode { get; set; } = null!;
    public string NewWhscode { get; set; } = null!;
    public string RelocatedBy { get; set; } = null!;
    public DateTime RelocatedDate { get; set; }
}
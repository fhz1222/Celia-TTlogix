
namespace Persistence.Entities;

public partial class TtStockTakeRelocationLog
{
    public string JobNo { get; set; } = null!;
    public string Pid { get; set; } = null!;
    /// <summary>
    /// 0: EXTRA; 1:MISSING
    /// </summary>
    public byte TransType { get; set; }
    public string OldLocationCode { get; set; } = null!;
    public string OldWhscode { get; set; } = null!;
    public string NewLocationCode { get; set; } = null!;
    public string NewWhscode { get; set; } = null!;
    public string RelocatedBy { get; set; } = null!;
    public DateTime RelocatedDate { get; set; }
}

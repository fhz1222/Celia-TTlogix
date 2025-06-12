
namespace Persistence.Entities;

public partial class TtStockTakeByLocDetail
{
    public string JobNo { get; set; } = null!;
    public string Pid { get; set; } = null!;
    public byte Status { get; set; }
    public string UploadedBy { get; set; } = null!;
    public DateTime? UoloadedDate { get; set; }
    public string LocationCode { get; set; } = null!;
}

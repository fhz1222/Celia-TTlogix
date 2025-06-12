using Domain.ValueObjects;

namespace Persistence.FilterHandlers.StockTakeTabs;

class StockTakeItemWithStorageInfo
{
    public string CustomerCode { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string SupplierID { get; set; } = null!;
    public string Pid { get; set; } = null!;
    public int Qty { get; set; }
    public string InJobNo { get; set; } = null!;
    public DateTime? InboundDate { get; set; }
    public string WhsCode { get; set; } = null!;
    public string LocationCode { get; set; } = null!;
    public byte? Status { get; set; } = null!;
    public string Remark { get; set; } = null!;
}

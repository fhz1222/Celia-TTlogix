using Domain.ValueObjects;

namespace Persistence.FilterHandlers.StockTakeTabs;

class GetStockTakeItemWithStorageInfoFilter
{
    public string JobNo { get; set; } = null!;
    public bool MustLocationCodeDiffer { get; set; }
    public bool MustWhsCodeMatch { get; set; }
    public List<StorageStatus>? InStatus { get; set; }
    public List<StorageStatus>? NotInStatus { get; set; }
}

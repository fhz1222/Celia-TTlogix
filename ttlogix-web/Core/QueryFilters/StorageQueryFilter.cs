using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class StorageQueryFilter
    {
        public string ProductCode { get; set; }
        public decimal? Qty { get; set; }
        public StorageStatus? Status { get; set; }
        public string OutJobNo { get; set; }
        public string LocationCode { get; set; }
    }
}

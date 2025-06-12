using TT.Core.Entities;

namespace TT.Core.QueryResults
{
    public class SFTStorageDetailWithPartInfoQueryResult
    {
        public StorageDetail StorageDetail { get; set; }
        public Location Location { get; set; }
        public int DecimalNum { get; set; }
        public string UOM { get; set; }
        public string Description { get; set; }
        public double DaysInStock { get; set; }
    }
}
using TT.Core.Entities;

namespace TT.Core.QueryResults
{
    public class StorageDetailWithPartInfoQueryResult
    {
        public StorageDetail StorageDetail { get; set; }
        public int DecimalNum { get; set; }
        public Location Location { get; set; }
        public string ExternalPID { get; set; }
        public string RefNo { get; set; }
        public decimal SPQ { get; set; }
    }
}
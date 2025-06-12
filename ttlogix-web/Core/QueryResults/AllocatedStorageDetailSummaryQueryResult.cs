using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class AllocatedStorageDetailSummaryQueryResult
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode { get; set; }
        public Ownership Ownership { get; set; }
        public string WHSCode { get; set; }
        public decimal AllocatedQty { get; set; }
        public decimal Qty { get; set; }
        public int AllocatedPkg { get; set; }
    }
}
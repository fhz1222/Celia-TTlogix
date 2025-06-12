using System;

namespace TT.Core.QueryFilters
{
    public class EStockTransferListQueryFilter : QueryFilterBase
    {
        public string UserWHSCode { get; set; }
        public string OrderNo { get; set; }
        public string FactoryID { get; set; }
        public string BlanketOrderNo { get; set; }
        public string SupplierID { get; set; }
        public string CompanyName { get; set; }
        public DateTime? ETAFrom { get; set; }
        public DateTime? ETATo { get; set; }
    }
}

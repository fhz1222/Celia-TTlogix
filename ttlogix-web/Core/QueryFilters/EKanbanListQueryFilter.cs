using System;

namespace TT.Core.QueryFilters
{
    public class EKanbanListQueryFilter : QueryFilterBase
    {
        public string UserWHSCode { get; set; }
        public string OrderNo { get; set; }
        public string FactoryId { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string BlanketOrderNo { get; set; }
        public DateTime? ETAFrom { get; set; }
        public DateTime? ETATo { get; set; }
        public DateTime? EDIDateFrom { get; set; }
        public DateTime? EDIDateTo { get; set; }
    }
}

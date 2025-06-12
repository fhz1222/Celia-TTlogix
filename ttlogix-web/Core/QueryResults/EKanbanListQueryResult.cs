using System;

namespace TT.Core.QueryResults
{
    public class EKanbanListQueryResult
    {
        public string OrderNo { get; set; }
        public string FactoryId { get; set; }
        public string BlanketOrderNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? EDIDate { get; set; }
    }
}
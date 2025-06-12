using System.Collections.Generic;
using TT.Core.Entities;

namespace TT.Core.QueryResults
{
    public class EKanbanDetailForPickingQueryResult
    {
        public IList<EKanbanDetail> EKanbanDetails { get; set; }
        public string OrderNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public string DropPoint { get; set; }
        public int? ExternalLineItem { get; set; }
        public decimal SumQty { get; set; }
        public decimal SumQtySupplied { get; set; }
        public int NoOfKanban { get; set; }
    }
}
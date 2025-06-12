namespace TT.Core.QueryResults
{
    public class KanbanListGroupQueryResult
    {
        public string OrderNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public decimal SumQtySupplied { get; set; }
        public decimal SumQtyReceived { get; set; }
        public int NoOfKanban { get; set; }
    }
}
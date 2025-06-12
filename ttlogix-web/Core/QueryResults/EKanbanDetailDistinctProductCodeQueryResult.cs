namespace TT.Core.QueryResults
{
    public class EKanbanDetailDistinctProductCodeQueryResult
    {
        public string OrderNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierId { get; set; }
        public decimal SumQty { get; set; }
        public decimal SumQtySupplied { get; set; }
        public decimal SumQtyReceived { get; set; }
        public int NoOfKanban { get; set; }
    }
}
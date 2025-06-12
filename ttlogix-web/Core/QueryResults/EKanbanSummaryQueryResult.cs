namespace TT.Core.QueryResults
{
    public class EKanbanSummaryQueryResult
    {
        public string OrderNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierId { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal TotalSupplied { get; set; }
    }

}
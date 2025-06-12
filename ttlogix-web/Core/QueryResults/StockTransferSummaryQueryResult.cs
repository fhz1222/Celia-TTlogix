namespace TT.Core.QueryResults
{
    public class StockTransferSummaryQueryResult
    {
        public string OrderNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public decimal Quantity { get; set; }
        public int Pkg { get; set; }
        public decimal PickedQty { get; set; }
        public int PickedPkg { get; set; }
    }

}
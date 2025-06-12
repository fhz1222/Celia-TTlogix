namespace TT.Core.QueryResults
{
    public class StockTransferDetailGroupListQueryResult
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode { get; set; }
        public decimal TotalQty { get; set; }
        public int TotalPkg { get; set; }
    }
}
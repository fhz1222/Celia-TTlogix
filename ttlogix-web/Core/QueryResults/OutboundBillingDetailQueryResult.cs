namespace TT.Core.QueryResults
{
    public class OutboundBillingDetailQueryResult
    {
        public string OutJobNo { get; set; }
        public string FactoryID { get; set; }
        public string OrderNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public string BillingNo { get; set; }
        public decimal Qty { get; set; }
    }
}
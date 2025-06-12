using System;

namespace TT.Core.QueryResults
{
    public class StockTransferDetailQueryResult
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public string PID { get; set; }
        public string ProductCode1 { get; set; }
        public string Description { get; set; }
        public string OriginalSupplierID { get; set; }
        public string OriginalWHSCode { get; set; }
        public string OriginalLocationCode { get; set; }
        public string SupplierID { get; set; }
        public string WHSCode { get; set; }
        public string LocationCode { get; set; }
        public string TransferredBy { get; set; }
        public DateTime? TransferredDate { get; set; }
        public decimal Qty { get; set; }
        public int DecimalNum { get; set; }
        public DateTime InboundDate { get; set; }
        public double DaysInStock { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
        public decimal PIDValue { get; set; }
    }

}
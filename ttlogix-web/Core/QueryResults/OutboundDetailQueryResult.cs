namespace TT.Core.QueryResults
{
    public class OutboundDetailQueryResult
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public decimal Qty { get; set; }
        public decimal PickedQty { get; set; }
        public long Pkg { get; set; }
        public long PickedPkg { get; set; }
        public byte Status { get; set; }
        public decimal DecimalNum { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal TotalSupplied { get; set; }
        public string UOM { get; set; }
        public bool IsCPart { get; set; }
        public decimal CPartSPQ { get; set; }
    }
}
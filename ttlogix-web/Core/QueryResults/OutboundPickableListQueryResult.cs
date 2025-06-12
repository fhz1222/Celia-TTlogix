namespace TT.Core.QueryResults
{
    public class OutboundPickableListQueryResult
    {
        public string SupplierID { get; set; }
        public string CustomerCode { get; set; }
        public string ProductCode1 { get; set; }
        public string ProductCode2 { get; set; }
        public string ProductCode3 { get; set; }
        public string Description { get; set; }
        public decimal SPQ { get; set; }
        public decimal CPartSPQ { get; set; }
        public bool IsCPart { get; set; }
        public int IsStandardPackaging { get; set; }
        public string UOM { get; set; }
        public decimal OnHandQty { get; set; }
        public decimal EHPQty { get; set; }
        public decimal SupplierQty { get; set; }
    }
}
namespace TT.Core.QueryResults
{
    public class InboundDetailQueryResult
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public string ProductCode { get; set; }
        public decimal Qty { get; set; }
        public int NoOfPackage { get; set; } = 1;
        public int NoOfLabel { get; set; } = 1;
        public string ControlCode1 { get; set; }
        public string ControlCode2 { get; set; }
        public string ControlCode3 { get; set; }
        public string ControlCode4 { get; set; }
        public string ControlCode5 { get; set; }
        public string ControlCode6 { get; set; }
        public string Remark { get; set; }
        public string CustomerCode { get; set; }
        public string WHSCode { get; set; }
        public string UOM { get; set; }
        public string UOMName { get; set; }
        public int DecimalNum { get; set; }
        public bool IsDefected { get; set; }
        public string Currency { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal LineValue { get; set; }
        public decimal ResidualValue { get; set; }
        public int PkgNo { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }

    }
}
using System;

namespace TT.Core.QueryResults
{
    public class PickingListDataToDownloadQueryResult
    {
        public string JobNo { get; set; }
        public string PID { get; set; }
        public string ProductCode1 { get; set; }
        public string ExternalID { get; set; }
        public string LocationCode { get; set; }
        public string WHSCode { get; set; }
        public string SupplierID { get; set; }
        public int? Version { get; set; }
        public DateTime InboundDate { get; set; }
        public string ProductionLine { get; set; }
        public byte IsPalletItem { get; set; }
        public string ControlCodeName { get; set; }
        public byte? ControlCodeType { get; set; }
        public string ControlCodeValue { get; set; }
        public DateTime? ControlDate { get; set; }
        public byte IsStandardPackaging { get; set; }
        public decimal SPQ { get; set; }
        public long Pkg { get; set; }
        public long PickedPkg { get; set; }
        public int DecimalNum { get; set; }
        public decimal TotalPickQty { get; set; }
        public decimal Qty { get; set; }
    }
}
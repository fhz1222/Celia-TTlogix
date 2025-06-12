using System;
using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class LoadingDetailQueryResult
    {
        public string JobNo { get; set; }
        public string OrderNo { get; set; }
        public string SupplierID { get; set; }
        public DateTime? ETD { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string OutJobNo { get; set; }
        public string CompanyName { get; set; }
        public OutboundStatus OutboundStatus { get; set; }
        public string CommInvNo { get; set; }
        public int? NoOfPallet { get; set; }
        public string KDSupplierID { get; set; }
        public string Currency { get; set; }
        public bool MixedCurrencies { get; set; }
        public int? NoOfPalletsEHP { get; set; }
        public int? NoOfPalletsSupplier { get; set; }
        public bool XDock { get; set; }
    }
}
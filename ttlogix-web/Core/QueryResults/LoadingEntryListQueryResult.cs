using System;
using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class LoadingEntryListQueryResult
    {
        public string OrderNo { get; set; }
        public string OutboundJobNo { get; set; }
        public string SupplierID { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime? ETD { get; set; }
        public OutboundStatus? OutboundStatus { get; set; }
        public string Remark { get; set; }
        public string TransportNo { get; set; }
    }
}
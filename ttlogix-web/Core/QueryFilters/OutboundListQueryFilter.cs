using System;
using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class OutboundListQueryFilter : QueryFilterBase
    {
        public string UserWHSCode { get; set; }
        public string JobNo { get; set; }
        public IEnumerable<string> CustomerCodes { get; set; }
        public string RefNo { get; set; }
        public OutboundType? TransType { get; set; }
        public IEnumerable<OutboundStatus> Statuses { get; set; }
        public string Remark { get; set; }
        public string SupplierName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DispatchedDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class InboundListQueryFilter : QueryFilterBase
    {
        public string UserWHSCode { get; set; }
        public string JobNo { get; set; }
        public IEnumerable<string> CustomerCodes { get; set; }
        public string CustomerName { get; set; }
        public string RefNo { get; set; }
        public string SupplierName { get; set; }
        public string ASNNumber { get; set; }
        public string ContainerNo { get; set; }
        public InboundType? TransType { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public IEnumerable<InboundStatus> Statuses { get; set; }
        public string Remark { get; set; }

    }
}

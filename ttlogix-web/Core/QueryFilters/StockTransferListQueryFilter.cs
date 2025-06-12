using System;
using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class StockTransferListQueryFilter : QueryFilterBase
    {
        public IEnumerable<string> CustomerCodes { get; set; }
        public string UserWHSCode { get; set; }
        public string JobNo { get; set; }
        public string CustomerName { get; set; }
        public string RefNo { get; set; }
        public StockTransferType? TransferType { get; set; }
        public IEnumerable<StockTransferStatus> Statuses { get; set; }
        public string Remark { get; set; }
        public string SupplierName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public StringFilterMode? RemarkFilter { get; set; }
    }

}

using System;
using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class StockTransferListQueryResult
    {
        public string JobNo { get; set; }
        public string CustomerCode { get; set; }
        public IEnumerable<string> SupplierNameList { get; set; }
        public string RefNo { get; set; }
        public string WHSCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public StockTransferType TransferType { get; set; }
        public StockTransferStatus Status { get; set; }
        public string Remark { get; set; }
    }
}
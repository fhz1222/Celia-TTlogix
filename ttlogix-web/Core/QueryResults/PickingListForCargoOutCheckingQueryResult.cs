using TT.Core.Entities;
using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class PickingListForCargoOutCheckingQueryResult
    {
        public PickingList PickingList { get; set; }
        public decimal? StockQty { get; set; }
        public decimal? StockAllocatedQty { get; set; }
        public string OutboundJobNo { get; set; }
        public StorageStatus? StockStatus { get; set; }
        public byte? InboundStatus { get; set; }
        public string InboundJobNo { get; set; }
        public string StockWhsCode { get; set; }
        public string StockPID { get; set; }
        public string CustomerCode { get; set; }
    }
}
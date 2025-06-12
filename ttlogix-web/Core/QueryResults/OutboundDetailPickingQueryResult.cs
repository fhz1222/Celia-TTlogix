using TT.Core.Entities;

namespace TT.Core.QueryResults
{
    public class OutboundDetailPickingQueryResult
    {
        public OutboundDetail OutboundDetail { get; set; }
        public decimal TotalPickedQty { get; set; }
        public int TotalPickedPkg { get; set; }
    }
}
using TT.Core.Entities;

namespace TT.Core.QueryResults
{
    public class CompleteOutboundQueryResult
    {
        public Outbound Outbound { get; set; }
        public PickingList PickingList { get; set; }
        public StorageDetail StorageDetail { get; set; }
        public Inbound Inbound { get; set; }
        public Inventory Inventory { get; set; }
    }
}
using System.Collections.Generic;

namespace TT.Services.Models
{
    public class SplitOutboundDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string JobNo { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public IEnumerable<PickingListItemId> PickingListItemIds { get; set; }
        public bool OwnershipSplit { get; set; }
    }
}

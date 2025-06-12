using System.Collections.Generic;

namespace TT.Services.Models
{
    public class CancelAllocationDto : AllocationDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public IEnumerable<PickingListItemId> ItemsToCancel { get; set; }
    }
}

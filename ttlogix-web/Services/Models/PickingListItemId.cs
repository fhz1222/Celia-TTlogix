using System.Collections.Generic;

namespace TT.Services.Models
{
    public class PickingListItemId
    {
        [Services.Utilities.RequiredAsJsonError]
        public string JobNo { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public int LineItem { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public int SeqNo { get; set; }

        public class PickingListItemIdEqualityComparer : IEqualityComparer<PickingListItemId>
        {
            public bool Equals(PickingListItemId x, PickingListItemId y)
            {
                return x?.JobNo == y?.JobNo && x?.LineItem == y?.LineItem && x?.SeqNo == y?.SeqNo;
            }

            public int GetHashCode(PickingListItemId obj)
            {
                return obj != null ? new { obj.JobNo, obj.LineItem, obj.SeqNo }.GetHashCode() : 0;
            }
        }
    }
}

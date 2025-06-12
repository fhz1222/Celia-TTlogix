using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class StorageDetailExtendedQueryFilter : StorageDetailQueryFilter
    {
        public bool AllocatedQtyGreaterThanZero { get; set; }
        public string OutJobNo { get; set; }
        public IEnumerable<LocationType> LocationTypes { get; set; }
        public IEnumerable<string> PIDs { get; set; }
        public bool LocationCodeNotEmpty { get; set; }
        public bool QtyGreaterThanZero { get; set; }
    }
}

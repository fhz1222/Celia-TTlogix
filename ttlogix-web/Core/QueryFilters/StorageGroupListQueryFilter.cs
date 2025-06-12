using System;
using System.Collections.Generic;
using System.Numerics;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class StorageGroupListQueryFilter : QueryFilterBase
    {
        public string GroupID { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? RepackedDate { get; set; }
        
        public StorageGroupStatus? Status { get; set; }

        public string InJobNo {get; set;}

    }
}

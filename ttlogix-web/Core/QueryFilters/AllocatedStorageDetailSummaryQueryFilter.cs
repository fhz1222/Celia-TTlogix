using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class AllocatedStorageDetailSummaryQueryFilter
    {
        public string ProductCode { get; set; }
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string WHSCode { get; set; }
    }
}

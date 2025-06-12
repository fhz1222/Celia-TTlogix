using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class SFTStorageDetailQueryFilter
    {
        public string WHSCode { get; set; }
        public string CustomerCode { get; set; }
        public IEnumerable<string> ProductCodes { get; set; }
        public IEnumerable<string> SupplierIds { get; set; }
        public string InJobNo { get; set; }
        public LocationType? LocationType { get; set; }
    }

}

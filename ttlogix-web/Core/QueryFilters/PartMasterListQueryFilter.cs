using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class PartMasterListQueryFilter : QueryFilterBase
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode1 { get; set; }
        public string ProductCode2 { get; set; }
        public string Description { get; set; }
        public string UomName { get; set; }
        public IEnumerable<ValueStatus> Statuses { get; set; }
        public bool? IsDefected { get; set; }
        public string iLogReadinessStatus { get; set; }
    }
}

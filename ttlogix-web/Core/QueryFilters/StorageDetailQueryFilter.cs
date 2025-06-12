using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class StorageDetailQueryFilter
    {
        public string CustomerCode { get; set; }
        public string ProductCode { get; set; }
        public string SupplierId { get; set; }
        public string WHSCode { get; set; }
        public Ownership? Ownership { get; set; }
        public decimal? QtyGreaterThan { get; set; }
        public IEnumerable<StorageStatus> Statuses { get; set; }
        public string InJobNo { get; set; }
        public LocationType? LocationType { get; set; }
        public string GroupID {get; set;}
        public string PID { get; set; }
    }
}

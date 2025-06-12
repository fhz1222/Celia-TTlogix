using System;
using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class LoadingListQueryFilter : QueryFilterBase
    {
        public string UserWHSCode { get; set; }
        public string JobNo { get; set; }
        public IEnumerable<string> CustomerCodes { get; set; }
        public string RefNo { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? TruckArrivalDate { get; set; }
        public DateTime? TruckDepartureDate { get; set; }
        public IEnumerable<LoadingStatus> Statuses { get; set; }
        public string Remark { get; set; }
        public string TruckLicencePlate { get; set; }
        public string TrailerNo { get; set; }
        public string DockNo { get; set; }
        public string TruckSeqNo { get; set; }
        public bool? AllowedForDispatch { get; set; }
        public StringFilterMode? RemarkFilter { get; set; }
    }
}

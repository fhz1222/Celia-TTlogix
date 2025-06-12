using System;
using TT.Core.Enums;
using TT.Core.Extensions;

namespace TT.Core.QueryResults
{
    public class LoadingListQueryResult
    {
        public string JobNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string RefNo { get; set; }
        public string WHSCode { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? TruckArrivalDate { get; set; }
        public DateTime? TruckDepartureDate { get; set; }
        public string Remark { get; set; }
        public LoadingStatus Status { get; set; }
        public OutboundStatus MinOutboundStatus { get; set; }
        public OutboundStatus MaxOutboundStatus { get; set; }
        public LoadingStatus CalculatedStatus => EntityExtensions.GetLoadingCalculatedStatus(MinOutboundStatus, MaxOutboundStatus);

        public string TruckLicencePlate { get; set; }
        public string TrailerNo { get; set; }
        public string DockNo { get; set; }
        public string TruckSeqNo { get; set; }
        public int NoOfPallet { get; set; }
        public bool AllowedForDispatch { get; set; }

    }

}
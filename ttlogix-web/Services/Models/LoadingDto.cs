using System;
using TT.Core.Enums;
using TT.Core.Extensions;

namespace TT.Services.Models
{
    public class LoadingDto : LoadingAddDto
    {
        public string JobNo { get; set; }
        public string CustomerName { get; set; }
        public LoadingStatus Status { get; set; }
        public string StatusString => Status.ToString();
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public int? NoOfPallet { get; set; }
        public DateTime? TruckArrivalDate { get; set; }
        public DateTime? TruckDepartureDate { get; set; }
        public DateTime? ETA { get; set; }
        public string Currency { get; set; }
        public bool MixedCurrencies { get; set; }
        public string TruckLicencePlate { get; set; }
        public string TrailerNo { get; set; }
        public string DockNo { get; set; }
        public string TruckSeqNo { get; set; }
        public bool AllowedForDispatch { get; set; }
        public int CalculatedNoOfPallet { get; set; }
        public OutboundStatus MinOutboundStatus { get; set; }
        public OutboundStatus MaxOutboundStatus { get; set; }
        public LoadingStatus CalculatedStatus => EntityExtensions.GetLoadingCalculatedStatus(MinOutboundStatus, MaxOutboundStatus);
        public string CalculatedStatusString => CalculatedStatus.ToString();
    }
}

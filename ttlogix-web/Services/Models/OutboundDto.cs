using System;
using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Services.Models
{
    public class OutboundDto
    {
        public string JobNo { get; set; }
        public string CustomerCode { get; set; }
        public string WHSCode { get; set; }
        public string OSNo { get; set; }
        public string RefNo { get; set; }
        public DateTime ETD { get; set; }
        public OutboundType TransType { get; set; }
        public byte Charged { get; set; }
        public string Remark { get; set; }
        public OutboundStatus Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string DispatchedBy { get; set; }
        public DateTime? DispatchedDate { get; set; }
        public string CommInvNo { get; set; }
        public int? NoOfPallet { get; set; }
        public int CalculatedNoOfPallet { get; set; }
        public string DeliveryTo { get; set; }
        public string NewWHSCode { get; set; }
        public bool XDock { get; set; }
        public IEnumerable<ReportPrintedDto> ReportsPrinted { get; set; }
        public bool AllowAutoallocation { get; set; }
        public string TransportNo { get; set; }
        public bool ShowOrderSummary { get; set; }
    }
}

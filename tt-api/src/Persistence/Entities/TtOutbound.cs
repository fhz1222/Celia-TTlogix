using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Persistence.Entities
{
    public partial class TtOutbound
    {
        public string JobNo { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public string WhsCode { get; set; } = null!;
        public string OSNo { get; set; } = string.Empty;
        public string RefNo { get; set; } = string.Empty;
        public DateTime ETD { get; set; }
        public byte TransType { get; set; }
        public byte Charged { get; set; }
        public string Remark { get; set; } = string.Empty;
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; } = string.Empty;
        public DateTime? RevisedDate { get; set; }
        public string CancelledBy { get; set; } = string.Empty;
        public DateTime? CancelledDate { get; set; }
        public string DispatchedBy { get; set; } = string.Empty;
        public DateTime? DispatchedDate { get; set; }
        public string CommInvNo { get; set; } = string.Empty;
        public int? NoOfPallet { get; set; } = 0;
        public string DeliveryTo { get; set; } = string.Empty;
        public string NewWHSCode { get; set; } = string.Empty;
        public bool XDock { get; set; }
    }
}

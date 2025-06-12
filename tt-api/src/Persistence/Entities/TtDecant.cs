using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtDecant
    {
        public string JobNo { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public string Whscode { get; set; } = null!;
        public string RefNo { get; set; } = null!;
        public string Remark { get; set; } = null!;
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string? ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}

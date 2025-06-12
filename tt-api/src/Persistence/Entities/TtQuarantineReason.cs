using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtQuarantineReason
    {
        public string Pid { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtPidCode
    {
        public string PidNo { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}

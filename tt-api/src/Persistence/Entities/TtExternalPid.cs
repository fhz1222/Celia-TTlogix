using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtExternalPid
    {
        public string Pid { get; set; } = null!;
        public string ExternalPid { get; set; } = null!;
        public byte ExternalSystem { get; set; }
        public string InJobNo { get; set; } = null!;
        public int InLineItem { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtArea
    {
        public string Code { get; set; } = null!;
        public string WhsCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public decimal Capacity { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}

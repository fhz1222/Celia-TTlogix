using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtWarehouse
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? PostCode { get; set; }
        public string? Country { get; set; }
        public string? Pic { get; set; }
        public string? TelNo { get; set; }
        public string? FaxNo { get; set; }
        public decimal Area { get; set; }
        public byte Type { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}

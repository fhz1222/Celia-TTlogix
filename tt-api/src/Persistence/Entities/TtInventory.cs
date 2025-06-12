using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtInventory
    {
        public string CustomerCode { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public string ProductCode1 { get; set; } = null!;
        public string Whscode { get; set; } = null!;
        public decimal OnHandQty { get; set; }
        public int? OnHandPkg { get; set; }
        public decimal AllocatedQty { get; set; }
        public int? AllocatedPkg { get; set; }
        public decimal QuarantineQty { get; set; }
        public int? QuarantinePkg { get; set; }
        public decimal? TransitQty { get; set; }
        public int? TransitPkg { get; set; }
        public decimal DiscrepancyQty { get; set; }
        public byte Ownership { get; set; }
    }
}

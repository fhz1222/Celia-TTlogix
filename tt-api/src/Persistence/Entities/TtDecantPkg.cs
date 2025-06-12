using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtDecantPkg
    {
        public string JobNo { get; set; } = null!;
        public string Pid { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public decimal Qty { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}

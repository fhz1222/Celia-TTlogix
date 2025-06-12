using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtInvTransactionPerSupplier
    {
        public string JobNo { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public DateTime JobDate { get; set; }
        public byte Act { get; set; }
        public decimal Qty { get; set; }
        public decimal BalanceQty { get; set; }
        public DateTime SystemDate { get; set; }
        public DateTime SystemDateTime { get; set; }
        public byte Ownership { get; set; }
    }
}

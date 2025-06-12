using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtInvTransactionPerWh
    {
        public string JobNo { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string Whscode { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public DateTime JobDate { get; set; }
        public byte Act { get; set; }
        public double Qty { get; set; }
        public long Pkg { get; set; }
        public double BalanceQty { get; set; }
        public long BalancePkg { get; set; }
        public DateTime? SystemDate { get; set; }
        public DateTime? SystemDateTime { get; set; }
    }
}

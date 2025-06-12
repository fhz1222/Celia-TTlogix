using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_InvTransaction")]
    public class InvTransaction
    {
        public string JobNo { get; set; }
        public string ProductCode { get; set; }
        public string CustomerCode { get; set; }
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



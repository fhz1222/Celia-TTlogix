using System;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_InvTransactionPerSupplier")]
    public class InvTransactionPerSupplier
    {
        public string JobNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public string CustomerCode { get; set; }
        public DateTime JobDate { get; set; }
        public byte Act { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal BalanceQty { get; set; }
        public DateTime SystemDate { get; set; }
        public DateTime SystemDateTime { get; set; }
        public Ownership Ownership { get; set; }
    }

}



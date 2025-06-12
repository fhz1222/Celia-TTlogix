using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_ShortfallCancelLog")]
    public class ShortfallCancelLog
    {
        public string JobNo { get; set; }
        public string PID { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }

}



using System;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_WHSTransferDetail")]
    public class WHSTransferDetail
    {
        public string JobNo { get; set; }
        public string PID { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
        public string OldWHSCode { get; set; }
        public string OldLocationCode { get; set; }
        public string NewWHSCode { get; set; }
        public string NewLocationCode { get; set; }
        public string TransferredBy { get; set; }
        public DateTime TransferredDate { get; set; }
        public Ownership? Ownership { get; set; }
    }
}



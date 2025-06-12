using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_StockTransferDetail")]
    public class StockTransferDetail
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public string PID { get; set; }
        public string OriginalSupplierID { get; set; }
        public string OriginalWHSCode { get; set; }
        public string OriginalLocationCode { get; set; }
        public string WHSCode { get; set; }
        public string LocationCode { get; set; }
        public string TransferredBy { get; set; }
        public DateTime? TransferredDate { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
    }

}



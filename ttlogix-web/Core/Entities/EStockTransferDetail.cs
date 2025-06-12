using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("EStockTransferDetail")]
    public class EStockTransferDetail
    {
        public string OrderNo { get; set; }
        public string ProductCode { get; set; }
        public string SerialNo { get; set; }
        public string SupplierID { get; set; }
        public string DropPoint { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantitySupplied { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityReceived { get; set; }
        public string BillingNo { get; set; }
        public int? ExternalLineItem { get; set; }
    }

}



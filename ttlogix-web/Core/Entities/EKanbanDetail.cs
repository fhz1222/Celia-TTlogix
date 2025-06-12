using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("EKanbanDetail")]
    public class EKanbanDetail
    {
        public string OrderNo { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string SupplierID { get; set; } = string.Empty;
        public string DropPoint { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantitySupplied { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityReceived { get; set; }
        public string BillingNo { get; set; } = string.Empty;
        public int? ExternalLineItem { get; set; }
    }

}

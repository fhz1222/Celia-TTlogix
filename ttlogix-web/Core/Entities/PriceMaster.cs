using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PriceMaster")]
    public class PriceMaster
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode1 { get; set; }
        public string Currency { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,6)")]
        public decimal BuyingPrice { get; set; }
        public string LastUpdatedInbound { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string RevisedBy { get; set; } = string.Empty;
        public DateTime? RevisedDate { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal SellingPrice { get; set; }
        public string LastUpdatedOutbound { get; set; }
        public string OutRevisedBy { get; set; } = string.Empty;
        public DateTime? OutRevisedDate { get; set; }
    }

}

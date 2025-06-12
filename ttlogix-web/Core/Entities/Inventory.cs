using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_Inventory")]
    public class Inventory
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode1 { get; set; }
        public string WHSCode { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal OnHandQty { get; set; }
        public int? OnHandPkg { get; set; } = 0;
        [Column(TypeName = "decimal(18,6)")]
        public decimal AllocatedQty { get; set; } = 0;
        public int? AllocatedPkg { get; set; } = 0;
        [Column(TypeName = "decimal(18,6)")]
        public decimal QuarantineQty { get; set; }
        public int? QuarantinePkg { get; set; } = 0;
        [Column(TypeName = "decimal(18,6)")]
        public decimal? TransitQty { get; set; } = 0;
        public int? TransitPkg { get; set; } = 0;
        [Column(TypeName = "decimal(18,6)")]
        public decimal DiscrepancyQty { get; set; }
        public Ownership Ownership { get; set; }
    }
}

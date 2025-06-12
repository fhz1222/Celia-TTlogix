

using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public partial class SupplierItemMaster
    {
        public string FactoryId { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public decimal PastCost { get; set; }
        public decimal CurrentCost { get; set; }
        public decimal FutureCost { get; set; }
        public string? PastCostCurrency { get; set; }
        public string? CurrentCostCurrency { get; set; }
        public string? FutureCostCurrency { get; set; }
        public DateTime? PastCostEffectiveDate { get; set; }
        public DateTime? CurrentCostEffectiveDate { get; set; }
        public DateTime? FutureCostEffectiveDate { get; set; }
    }
}



using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public partial class BillingLog
    {
        public string JobNo { get; set; } = null!;
        public string FactoryId { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string RefNo { get; set; } = null!;
        public string? CostCurrency { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal Quantity { get; set; }
        public string BillingNo { get; set; } = null!;
    }
}

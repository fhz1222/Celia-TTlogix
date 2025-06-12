using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public partial class TtOutboundDetail
    {
        public string JobNo { get; set; } = null!;
        public int LineItem { get; set; }
        public string ProductCode { get; set; } = null!;
        public string SupplierID { get; set; } = null!;
        public decimal Qty { get; set; }
        public decimal PickedQty { get; set; }
        public long Pkg { get; set; }
        public long PickedPkg { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; } = string.Empty;
        public DateTime? RevisedDate { get; set; }
        public decimal? CostPrice { get; set; } = 0;
        public string CostCurrency { get; set; } = string.Empty;
        public string BillingReportNo { get; set; } = string.Empty;
    }
}

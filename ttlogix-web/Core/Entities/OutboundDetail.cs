using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_OutboundDetail")]
    public class OutboundDetail
    {
        [Column(TypeName="varchar(15)")]
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        [Column(TypeName="varchar(30)")]
        public string ProductCode { get; set; }
        [Column(TypeName="varchar(10)")]
        public string SupplierID { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal PickedQty { get; set; }
        public long Pkg { get; set; }
        public long PickedPkg { get; set; }
        public byte Status { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string RevisedBy { get; set; } = string.Empty;
        public DateTime? RevisedDate { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? CostPrice { get; set; } = 0;
        [Column(TypeName="varchar(5)")]
        public string CostCurrency { get; set; } = string.Empty;
        [Column(TypeName="varchar(20)")]
        public string BillingReportNo { get; set; } = string.Empty;

    }

}


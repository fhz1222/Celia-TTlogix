using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("EPO")]
    public class EPO
    {
        public string PONo { get; set; }
        public string POLineItem { get; set; }
        public string SupplierID { get; set; }
        public string FactoryID { get; set; }
        public string ProductCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Qty { get; set; }
        public byte? Status { get; set; }
        public int? Revision { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string LastUpdateEDI { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? TolerancePercentage { get; set; }
        public byte? UnlimitedFlag { get; set; }
        public string SAPLocationID { get; set; }
    }

}

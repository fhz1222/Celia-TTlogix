using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("BillingLog")]
    public class BillingLog
    {
        public string JobNo { get; set; }
        public string FactoryID { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode { get; set; }
        public string RefNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CostCurrency { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? CostPrice { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Quantity { get; set; }
        public byte Billed { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BillingNo { get; set; }
    }

}



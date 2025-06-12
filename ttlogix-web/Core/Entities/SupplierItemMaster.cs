using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("SupplierItemMaster")]
    public class SupplierItemMaster
    {
        [Required]
        public string FactoryID { get; set; }
        [Required]
        public string SupplierID { get; set; }

        [Required]
        public string ProductCode { get; set; }
        [Required]
        public string SupplierPartNo { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal PastCost { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal CurrentCost { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal FutureCost { get; set; }
        public string PastCostCurrency { get; set; }
        public string CurrentCostCurrency { get; set; }
        public string FutureCostCurrency { get; set; }
        public DateTime? PastCostEffectiveDate { get; set; }
        public DateTime? CurrentCostEffectiveDate { get; set; }
        public DateTime? FutureCostEffectiveDate { get; set; }
        public int TargetMinStockDays { get; set; }
        public int TargetMinStockQty { get; set; }
        [Required]
        public string TargetMinStockQtyStatus { get; set; }
        public int TargetMaxStockDays { get; set; }
        public int? TargetMaxStockQty { get; set; }
        public string TargetMaxStockQtyStatus { get; set; }
        public int MinShipQty { get; set; }
        public int OuterQty { get; set; }
        public int InnerQty { get; set; }
        public string DrawingNumberComments { get; set; }
        public int LeadTimeMRPRunToExSupplier { get; set; }
        public int LeadTimeExSupplierToShipDepart { get; set; }
        public int ShipTransitTime { get; set; }
        public int LeadTimePortArrivalToWH { get; set; }
        public int TotalOverseasLeadTime { get; set; }
        public int LeadTimeOrderToDispatch { get; set; }
        public int LocalTransitTime { get; set; }
        public int TotalLocalLeadTime { get; set; }
        public byte SupplierStatus { get; set; }
        [Required]
        public string DTL { get; set; }
    }

}




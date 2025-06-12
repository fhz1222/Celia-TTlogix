using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("SupplierDetail")]
    public class SupplierDetail
    {
        public string SupplierID { get; set; }
        public string FactoryID { get; set; }
        public string EmergencyContact { get; set; } = string.Empty;
        public int OverStockAlertTolerance { get; set; }
        public int MaxShipLimitPercentage { get; set; }
        public int LateASNTolerance { get; set; }
        public string InvoiceFrequency { get; set; } = string.Empty;
        public int MinimumStockTolerance { get; set; }
        public int LateVesselBookingTolerance { get; set; }
        public int ForecastAnalysis { get; set; }
        public int? MRPChangeDays { get; set; }
        public int? OLead1A { get; set; }
        public int? OLead2A { get; set; }
        public int? OLead3A { get; set; }
        public int? OLead4A { get; set; }
        public int? LLead1A { get; set; }
        public int? LLead2A { get; set; }
        public int? LLead3A { get; set; }
        public int? LLead4A { get; set; }
        public int? OLead1B { get; set; }
        public int? OLead2B { get; set; }
        public int? OLead3B { get; set; }
        public int? OLead4B { get; set; }
        public int? LLead1B { get; set; }
        public int? LLead2B { get; set; }
        public int? LLead3B { get; set; }
        public int? LLead4B { get; set; }
        public int? OLead1C { get; set; }
        public int? OLead2C { get; set; }
        public int? OLead3C { get; set; }
        public int? OLead4C { get; set; }
        public int? LLead1C { get; set; }
        public int? LLead2C { get; set; }
        public int? LLead3C { get; set; }
        public int? LLead4C { get; set; }
        public int? OLead1X { get; set; }
        public int? OLead2X { get; set; }
        public int? OLead3X { get; set; }
        public int? OLead4X { get; set; }
        public int? LLead1X { get; set; }
        public int? LLead2X { get; set; }
        public int? LLead3X { get; set; }
        public int? LLead4X { get; set; }
        public int? CommitFinishedGoods { get; set; }
        public int? CommitWIP { get; set; }
        public int? CommitRawMaterials { get; set; }
        public string AgreementCode { get; set; }
        public string DelFreqA { get; set; }
        public string DelFreqB { get; set; }
        public string DelFreqC { get; set; }
        public string DelFreqX { get; set; }
        public string LeadTimeText { get; set; }
        public string FrequencyText { get; set; }
        public int? DefaultSunsetPeriod { get; set; }
    }

}



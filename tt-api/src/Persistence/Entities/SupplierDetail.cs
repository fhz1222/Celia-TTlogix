namespace Persistence.Entities;

public class SupplierDetail
{
    public string SupplierId { get; set; } = null!;
    public string FactoryId { get; set; } = null!;
    public string EmergencyContact { get; set; } = null!;
    public int OverStockAlertTolerance { get; set; }
    public int MaxShipLimitPercentage { get; set; }
    public int LateAsntolerance { get; set; }
    public string InvoiceFrequency { get; set; } = null!;
    public int MinimumStockTolerance { get; set; }
    public int LateVesselBookingTolerance { get; set; }
    public int ForecastAnalysis { get; set; }
    public int? MrpchangeDays { get; set; }
    public int? Olead1A { get; set; }
    public int? Olead2A { get; set; }
    public int? Olead3A { get; set; }
    public int? Olead4A { get; set; }
    public int? Llead1A { get; set; }
    public int? Llead2A { get; set; }
    public int? Llead3A { get; set; }
    public int? Llead4A { get; set; }
    public int? Olead1B { get; set; }
    public int? Olead2B { get; set; }
    public int? Olead3B { get; set; }
    public int? Olead4B { get; set; }
    public int? Llead1B { get; set; }
    public int? Llead2B { get; set; }
    public int? Llead3B { get; set; }
    public int? Llead4B { get; set; }
    public int? Olead1C { get; set; }
    public int? Olead2C { get; set; }
    public int? Olead3C { get; set; }
    public int? Olead4C { get; set; }
    public int? Llead1C { get; set; }
    public int? Llead2C { get; set; }
    public int? Llead3C { get; set; }
    public int? Llead4C { get; set; }
    public int? Olead1X { get; set; }
    public int? Olead2X { get; set; }
    public int? Olead3X { get; set; }
    public int? Olead4X { get; set; }
    public int? Llead1X { get; set; }
    public int? Llead2X { get; set; }
    public int? Llead3X { get; set; }
    public int? Llead4X { get; set; }
    public int? CommitFinishedGoods { get; set; }
    public int? CommitWip { get; set; }
    public int? CommitRawMaterials { get; set; }
    public string? AgreementCode { get; set; }
    public string? DelFreqA { get; set; }
    public string? DelFreqB { get; set; }
    public string? DelFreqC { get; set; }
    public string? DelFreqX { get; set; }
    public string? LeadTimeText { get; set; }
    public string? FrequencyText { get; set; }
    public int? DefaultSunsetPeriod { get; set; }
    public int? AirfreightToPort { get; set; }
    public int? AirfreightToDest { get; set; }
    public int InvoiceBatchSequenceNo { get; set; }
}

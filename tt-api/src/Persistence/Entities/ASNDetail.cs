namespace Persistence.Entities;

public class ASNDetail
{
    public string Asnno { get; set; } = null!;
    public int LineItem { get; set; }
    public string ProductCode { get; set; } = null!;
    public string? OrderNo { get; set; }
    public string ContainerNo { get; set; } = null!;
    public string? ContainerSize { get; set; }
    public string SealNo { get; set; } = null!;
    public DateTime? ManufacturedDate { get; set; }
    public string? BatchNo { get; set; }
    public int QtyPerOuter { get; set; }
    public int NoOfOuter { get; set; }
    public bool Breakpoint { get; set; }
    public string InJobNo { get; set; } = null!;
    public string? BillOfLading { get; set; }
    public string? MaerskSono { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? ExSupplierDate { get; set; }
    public DateTime? ShipDepartureDate { get; set; }
    public DateTime? PortArrivalDate { get; set; }
    public DateTime? StoreArrivalDate { get; set; }
    public string? VesselName { get; set; }
    public string? VoyageNo { get; set; }
    public string Status { get; set; } = null!;
    public string PreImportStatus { get; set; } = null!;
    public string? Pono { get; set; }
    public string? PolineNo { get; set; }
    public bool IsSapschedulingAgreement { get; set; }
}

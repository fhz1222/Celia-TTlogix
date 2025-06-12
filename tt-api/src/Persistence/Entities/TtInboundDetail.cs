namespace Persistence.Entities;

public class TtInboundDetail
{
    public string JobNo { get; set; } = null!;
    public int LineItem { get; set; }
    public int PkgLineItem { get; set; }
    public string ProductCode { get; set; } = null!;
    public decimal ImportedQty { get; set; }
    public decimal Qty { get; set; }
    public int NoOfPackage { get; set; }
    public string PackageType { get; set; } = null!;
    public int NoOfLabel { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public string ControlCode1 { get; set; } = null!;
    public string ControlCode2 { get; set; } = null!;
    public string ControlCode3 { get; set; } = null!;
    public string ControlCode4 { get; set; } = null!;
    public string ControlCode5 { get; set; } = null!;
    public string ControlCode6 { get; set; } = null!;
    public DateTime? ControlDate { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisedDate { get; set; }
    public string? Asnno { get; set; }
    public int? AsnlineItem { get; set; }
    public string? Remark { get; set; }
    public decimal? BuyingPricePerLine { get; set; }
}

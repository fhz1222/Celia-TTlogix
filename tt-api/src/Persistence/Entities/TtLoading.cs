namespace Persistence.Entities;

public partial class TtLoading
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string Whscode { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string Remark { get; set; } = null!;
    public byte Status { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? ConfirmedBy { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public DateTime? Etd { get; set; }
    public int? NoOfPallet { get; set; }
    public DateTime? TruckArrivalDate { get; set; }
    public DateTime? TruckDepartureDate { get; set; }
    public DateTime? Eta { get; set; }
    public string? TruckLicencePlate { get; set; }
    public string? TrailerNo { get; set; }
    public string? DockNo { get; set; }
    public string? TruckSeqNo { get; set; }
    public bool AllowedForDispatch { get; set; }
    public DateTime? AllowedForDispatchModifiedDate { get; set; }
    public string? AllowedForDispatchModifiedBy { get; set; }
}

using Domain.ValueObjects;

namespace Domain.Entities;

public class AddressContact
{
    public string Code { get; set; } = null!;
    public string AddressCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? TelNo { get; set; }
    public string? FaxNo { get; set; }
    public string Email { get; set; } = null!;
    public Status Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisedDate { get; set; }
}

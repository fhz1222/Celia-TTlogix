using Domain.ValueObjects;

namespace Domain.Entities;

public class CustomerClient
{
    public string Code { get; set; } = null!;
    public string CompanyCode { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PrimaryAddress { get; set; } = null!;
    public string BillingAddress { get; set; } = null!;
    public string ShippingAddress { get; set; } = null!;
    public string Pic1 { get; set; } = null!;
    public string Pic2 { get; set; } = null!;
    public CustomerClientStatus Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisedDate { get; set; }
}

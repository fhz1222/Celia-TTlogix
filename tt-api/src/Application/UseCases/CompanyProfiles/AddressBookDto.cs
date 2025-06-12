using Domain.ValueObjects;

namespace Application.UseCases.CompanyProfiles;

public class AddressBookDto
{
    public string Code { get; set; } = null!;
    public string CompanyCode { get; set; } = null!;
    public Status Status { get; set; } = null!;
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public string? TelNo { get; set; }
    public string? FaxNo { get; set; }
}
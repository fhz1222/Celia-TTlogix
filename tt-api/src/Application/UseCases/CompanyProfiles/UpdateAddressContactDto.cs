namespace Application.UseCases.CompanyProfiles;

public class UpdateAddressContactDto
{
    public string Code { get; set; } = null!;
    public string AddressCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? TelNo { get; set; }
    public string? FaxNo { get; set; }
    public string Email { get; set; } = null!;
}
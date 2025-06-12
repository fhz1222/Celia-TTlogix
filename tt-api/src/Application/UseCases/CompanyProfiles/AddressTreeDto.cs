using Domain.ValueObjects;

namespace Application.UseCases.CompanyProfiles;

public class AddressTreeDto
{
    public class AddressTreeContactDto
    {
        public string Code { get; set; } = null!;
        public string AddressCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? TelNo { get; set; }
        public string? FaxNo { get; set; }
        public string Email { get; set; } = null!;
        public Status Status { get; set; } = null!;
    }

    public class AddressTreeBookDto
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
        public IEnumerable<AddressTreeContactDto> AddressContacts { get; set; } = null!;
    }

    public class AddressTreeCompanyProfileDto
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Status Status { get; set; } = null!;
        public IEnumerable<AddressTreeBookDto> AddressBooks { get; set; } = null!;
    }

    public IEnumerable<AddressTreeCompanyProfileDto> CompanyProfiles { get; set; } = null!;
}



using Domain.ValueObjects;

namespace Application.UseCases.CompanyProfiles.Queries.GetCompanyProfiles;

public class GetCompanyProfilesDtoFilter
{
    public string? Code { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public Status? Status { get; set; }
}

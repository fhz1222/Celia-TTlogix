using Domain.ValueObjects;

namespace Application.UseCases.Registration.Queries.GetLocationList;

public class GetLocationListDtoFilter
{
    public string? Code { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? WhsCode { get; set; }
    public int? Type { get; set; }
    public int? IsPriority { get; set; }
    public string? AreaCode { get; set; }
    public int? Status { get; set; }
    public int? ILogLocationCategoryId { get; set; } = null!;
}

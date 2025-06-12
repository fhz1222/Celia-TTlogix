using Domain.ValueObjects;

namespace Application.UseCases.Registration.Queries.GetProductCodeList;

public class GetProductCodeListDtoFilter
{
    public string? Code { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public int? Type { get; set; }
    public int? Status { get; set; }
}

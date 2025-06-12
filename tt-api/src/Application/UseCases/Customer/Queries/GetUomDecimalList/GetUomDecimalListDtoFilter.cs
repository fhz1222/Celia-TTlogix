using Domain.ValueObjects;

namespace Application.UseCases.Customer.Queries.GetUomDecimalList;

public class GetUomDecimalListDtoFilter
{
    public string CustomerCode { get; set; } = null!;
    public string? Name { get; set; } = null!;

    public DtoFilterIntRange? DecimalNum { get; set; } = null!;
    public UomDecimalStatus? Status { get; set; }
}

using Domain.ValueObjects;

namespace Application.UseCases.Customer;

public class UomDecimalListItemDto
{
    public string CustomerCode { get; set; } = null!;
    public string Uom { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int DecimalNum { get; set; }
    public UomDecimalStatus Status { get; set; } = null!;
}
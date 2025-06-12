using Domain.ValueObjects;

namespace Application.UseCases.Registration;

public class AreaListItemDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public Status Status { get; set; } = null!;
}

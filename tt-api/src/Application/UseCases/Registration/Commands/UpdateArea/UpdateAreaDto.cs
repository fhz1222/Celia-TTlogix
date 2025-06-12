using Domain.ValueObjects;

namespace Application.UseCases.Registration.Commands.UpdateArea;

public class UpdateAreaDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public decimal Capacity { get; set; }
    public Status Status { get; set; } = null!;
}

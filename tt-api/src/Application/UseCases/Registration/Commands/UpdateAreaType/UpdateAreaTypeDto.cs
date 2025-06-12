using Domain.ValueObjects;

namespace Application.UseCases.Registration.Commands.UpdateAreaType;

public class UpdateAreaTypeDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Status Status { get; set; } = null!;
}

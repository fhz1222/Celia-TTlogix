using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogLocationIntegration.Commands.DeactivateLocationsCommand;

public class DeactivateLocationsCommand : IRequest
{
    public IEnumerable<ILogIntegrationLocationIdDto> Items { get; set; } = null!;
}

public class DeactivateLocationsCommandHandler : IRequestHandler<DeactivateLocationsCommand>
{
    private readonly IRepository repository;

    public DeactivateLocationsCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(DeactivateLocationsCommand request, CancellationToken cancellationToken)
    {
        repository.Locations.DeactivateLocations(request.Items);
        await repository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

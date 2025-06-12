using Application.Interfaces.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.ILogLocationIntegration.Commands.ActivateLocationsCommand;

public class ActivateLocationsCommand : IRequest
{
    public IEnumerable<ILogIntegrationLocationIdDto> Items { get; set; } = null!;
}

public class ActivateLocationsCommandHandler : IRequestHandler<ActivateLocationsCommand>
{
    private readonly IRepository repository;

    public ActivateLocationsCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(ActivateLocationsCommand request, CancellationToken cancellationToken)
    {
        repository.Locations.ActivateLocations(request.Items);
        await repository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogLocationIntegration.Commands.UpdateLocationsCommand;

public class UpdateLocationsCommand : IRequest
{
    public IEnumerable<ILogIntegrationLocationDto> Items { get; set; } = null!;
}

public class UpdateLocationsCommandHandler : IRequestHandler<UpdateLocationsCommand>
{
    private readonly IRepository repository;

    public UpdateLocationsCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(UpdateLocationsCommand request, CancellationToken cancellationToken)
    {
        repository.Locations.UpdateLocations(request.Items);
        await repository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

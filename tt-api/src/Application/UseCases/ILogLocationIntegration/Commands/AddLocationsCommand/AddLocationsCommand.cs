using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogLocationIntegration.Commands.AddLocationsCommand;

public class AddLocationsCommand : IRequest
{
    public IEnumerable<ILogIntegrationLocationDto> Items { get; set; } = null!;
}

public class AddLocationsCommandHandler : IRequestHandler<AddLocationsCommand>
{
    private readonly IRepository repository;

    public AddLocationsCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(AddLocationsCommand request, CancellationToken cancellationToken)
    {
        repository.Locations.AddLocations(request.Items);
        await repository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

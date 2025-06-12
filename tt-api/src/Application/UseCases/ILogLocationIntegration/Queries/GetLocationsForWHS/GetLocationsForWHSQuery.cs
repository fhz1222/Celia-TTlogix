using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogLocationIntegration.Queries.GetLocationsForWHS;

public class GetLocationsForWHSQuery : IRequest<IEnumerable<ILogIntegrationLocationDto>>
{
    public string[] WHSCodes;
}

public class GetLocationsForWHSQueryHandler : IRequestHandler<GetLocationsForWHSQuery, IEnumerable<ILogIntegrationLocationDto>>
{
    private readonly IRepository repository;

    public GetLocationsForWHSQueryHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IEnumerable<ILogIntegrationLocationDto>> Handle(GetLocationsForWHSQuery request, CancellationToken cancellationToken)
    {
        var locations = repository.Locations.GetLocationsForWHS(request.WHSCodes);
        return locations;
    }
}


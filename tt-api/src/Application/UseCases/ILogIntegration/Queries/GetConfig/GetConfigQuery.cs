using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogIntegration.Queries.GetConfig;

public class GetConfigQuery : IRequest<IntegrationConfigDto>
{

}

public class GetConfigQueryHandler : IRequestHandler<GetConfigQuery, IntegrationConfigDto>
{
    private readonly IILogIntegrationRepository ilogRepository;

    public GetConfigQueryHandler(IILogIntegrationRepository ilogRepository)
    {
        this.ilogRepository = ilogRepository;
    }

    public Task<IntegrationConfigDto> Handle(GetConfigQuery request, CancellationToken cancellationToken)
    {
        var warehouses = ilogRepository.GetWarehouses();
        var isEnabled = ilogRepository.GetStatus();

        var result = new IntegrationConfigDto()
        {
            WHSCodes = warehouses,
            IsEnabled = isEnabled
        };

        return Task.FromResult(result);
    }
}

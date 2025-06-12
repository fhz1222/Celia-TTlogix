using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogIntegration.Queries.IsActiveForWarehouse;

public class IsActiveForWarehouseQuery : IRequest<bool>
{
    public string WHSCode { get; set; }
}

public class IsEnabledForWarehouseQueryHandler : IRequestHandler<IsActiveForWarehouseQuery, bool>
{
    private readonly IILogIntegrationRepository iLogIntegrationRepository;

    public IsEnabledForWarehouseQueryHandler(IILogIntegrationRepository iLogIntegrationRepository)
    {
        this.iLogIntegrationRepository = iLogIntegrationRepository;
    }

    public Task<bool> Handle(IsActiveForWarehouseQuery request, CancellationToken cancellationToken)
    {
        var isEnabled = iLogIntegrationRepository.GetStatus();
        var isWarehouseIntegrated = iLogIntegrationRepository.GetWarehouses().Contains(request.WHSCode);

        return Task.FromResult(isEnabled && isWarehouseIntegrated);
    }
}
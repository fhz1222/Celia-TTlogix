using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.GetFactories;

public class GetWarehouseFactoriesQuery : IRequest<List<FactoryDto>>
{
    public string WarehouseCode { get; set; } = default!;
}

public class GetWarehouseFactoriesQueryHandler : IRequestHandler<GetWarehouseFactoriesQuery, List<FactoryDto>>
{
    private readonly IInvoiceRequestRepository repository;

    public GetWarehouseFactoriesQueryHandler(IInvoiceRequestRepository repository) => this.repository = repository;

    public async Task<List<FactoryDto>> Handle(GetWarehouseFactoriesQuery request, CancellationToken cancellationToken)
    {
        var factories = await repository.GetFactoriesForWarehouse(request.WarehouseCode);
        return factories;
    }
}
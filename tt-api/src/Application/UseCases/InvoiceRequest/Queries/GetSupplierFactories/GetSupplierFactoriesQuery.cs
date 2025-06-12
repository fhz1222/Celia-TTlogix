using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.GetSupplierFactories;

public class GetSupplierFactoriesQuery : IRequest<List<FactoryDto>>
{
    public string SupplierId { get; set; } = default!;
}

public class GetFactoriesQueryHandler : IRequestHandler<GetSupplierFactoriesQuery, List<FactoryDto>>
{
    private readonly IInvoiceRequestRepository repository;

    public GetFactoriesQueryHandler(IInvoiceRequestRepository repository) => this.repository = repository;

    public async Task<List<FactoryDto>> Handle(GetSupplierFactoriesQuery request, CancellationToken cancellationToken)
    {
        var factories = await repository.GetFactoriesForSupplier(request.SupplierId);
        return factories;
    }
}

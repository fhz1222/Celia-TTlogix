using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Supplier.Queries.GetSuppliers;

public class GetSuppliersQuery : IRequest<IEnumerable<SupplierDto>>
{
    public SupplierDtoFilter filter { get; set; } = null!;
}

public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, IEnumerable<SupplierDto>>
{
    private readonly ISupplierRepository SupplierRepository;

    public GetSuppliersQueryHandler(ISupplierRepository SupplierRepository)
    {
        this.SupplierRepository = SupplierRepository;
    }

    public Task<IEnumerable<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<SupplierDto> result = SupplierRepository.GetSuppliers(request.filter);
        return Task.FromResult(result);
    }
}

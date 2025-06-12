using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Customer.Queries.GetInventoryControl;

public class GetInventoryControlQuery : IRequest<InventoryControlDto>
{
    public string CustomerCode { get; set; } = null!;
}

public class GetInventoryControlHandler : IRequestHandler<GetInventoryControlQuery, InventoryControlDto>
{
    private readonly ICustomerRepository customerRepository;

    public GetInventoryControlHandler(ICustomerRepository customerRepository)
    {
        this.customerRepository = customerRepository;
    }

    public Task<InventoryControlDto> Handle(GetInventoryControlQuery request, CancellationToken cancellationToken)
    {
        var result = customerRepository.GetInventoryControl<InventoryControlDto>(request.CustomerCode);
        return Task.FromResult(result);
    }
}

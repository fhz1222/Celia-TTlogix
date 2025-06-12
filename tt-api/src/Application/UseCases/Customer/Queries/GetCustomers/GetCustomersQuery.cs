using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Customer.Queries.GetCustomers;

public class GetCustomersQuery : IRequest<IEnumerable<CustomerDto>>
{

}

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        this.customerRepository = customerRepository;
    }

    public Task<IEnumerable<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<CustomerDto> result = customerRepository.GetCustomers();
        return Task.FromResult(result);
    }
}

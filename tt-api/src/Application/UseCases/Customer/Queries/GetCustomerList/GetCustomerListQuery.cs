using Application.Common.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Customer.Queries.GetCustomerList;

public class GetCustomerListQuery : IRequest<List<CustomerListItemDto>>
{
    public GetCustomerListDtoFilter Filter { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetCustomerListQueryHandler : IRequestHandler<GetCustomerListQuery, List<CustomerListItemDto>>
{
    private readonly ICustomerRepository customerRepository;

    public GetCustomerListQueryHandler(ICustomerRepository customerRepository)
    {
        this.customerRepository = customerRepository;
    }

    public async Task<List<CustomerListItemDto>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
    {
        var result = customerRepository.GetCustomerList(request.Filter, request.OrderByExpression, request.OrderByDescending);
        return await Task.FromResult(result);
    }
}

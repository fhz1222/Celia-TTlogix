using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Customer.Queries.GetCustomerClientList;

public class GetCustomerClientListQuery : IRequest<List<CustomerClientListItemDto>>
{
    public GetCustomerClientListDtoFilter Filter { get; set; } = null!;
    public string? OrderBy { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetCustomerClientListQueryHandler : IRequestHandler<GetCustomerClientListQuery, List<CustomerClientListItemDto>>
{
    private readonly ICustomerRepository customerRepository;

    public GetCustomerClientListQueryHandler(ICustomerRepository customerRepository)
    {
        this.customerRepository = customerRepository;
    }

    public async Task<List<CustomerClientListItemDto>> Handle(GetCustomerClientListQuery request, CancellationToken cancellationToken)
    {
        var result = customerRepository.GetCustomerClientList(request.Filter, request.OrderBy, request.OrderByDescending);
        return await Task.FromResult(result);
    }
}

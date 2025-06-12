using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Customer.Queries.GetUomDecimalList;

public class GetUomDecimalListQuery : IRequest<List<UomDecimalListItemDto>>
{
    public GetUomDecimalListDtoFilter Filter { get; set; } = null!;
    public string? OrderBy { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetUomDecimalListQueryHandler : IRequestHandler<GetUomDecimalListQuery, List<UomDecimalListItemDto>>
{
    private readonly ICustomerRepository customerRepository;

    public GetUomDecimalListQueryHandler(ICustomerRepository customerRepository)
    {
        this.customerRepository = customerRepository;
    }

    public async Task<List<UomDecimalListItemDto>> Handle(GetUomDecimalListQuery request, CancellationToken cancellationToken)
    {
        var result = customerRepository.GetUomDecimalList(request.Filter, request.OrderBy, request.OrderByDescending);
        return await Task.FromResult(result);
    }
}

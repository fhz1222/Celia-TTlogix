using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.GetOpenRequests;

public class GetOpenRequestsQuery : IRequest<List<JobDto>>
{
    public string FactoryId { get; set; } = default!;
    public string SupplierId { get; set; } = default!;
}

public class GetOpenRequestsQueryHandler : IRequestHandler<GetOpenRequestsQuery, List<JobDto>>
{
    private readonly IInvoiceRequestRepository repository;

    public GetOpenRequestsQueryHandler(IInvoiceRequestRepository repository) => this.repository = repository;

    public async Task<List<JobDto>> Handle(GetOpenRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await repository.GetOpenRequests(request.FactoryId, request.SupplierId);
        return requests;
    }
}

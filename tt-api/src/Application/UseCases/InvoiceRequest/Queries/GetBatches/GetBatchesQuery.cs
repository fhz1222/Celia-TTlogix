using Application.Common.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.GetBatches;

public class GetBatchesQuery : IRequest<PaginatedList<InvoiceBatchDto>>
{
    public GetBatchesQueryFilter Filter { get; set; } = null!;
    public PaginationQuery Pagination { get; set; } = default!;
}

public class GetBatchesQueryHandler : IRequestHandler<GetBatchesQuery, PaginatedList<InvoiceBatchDto>>
{
    private readonly IInvoiceRequestRepository repository;

    public GetBatchesQueryHandler(IInvoiceRequestRepository repository)
    {
        this.repository = repository;
    }

    public async Task<PaginatedList<InvoiceBatchDto>> Handle(GetBatchesQuery request, CancellationToken cancellationToken)
    {
        var data = await repository.GetBatches(request.Filter, request.Pagination);
        return data;
    }
}

using Application.Common.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InboundReversals.Queries.GetReversibleInbounds;

public class GetReversibleInboundsQuery : IRequest<PaginatedList<ReversibleInboundDto>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string? InJobNo { get; set; } = null!;
    public DateTime? NewerThan { get; set; }
}

public class GetReversibleInboundsQueryHandler : IRequestHandler<GetReversibleInboundsQuery, PaginatedList<ReversibleInboundDto>>
{
    private readonly IInboundReversalRepository inboundReversalRepository;

    public GetReversibleInboundsQueryHandler(IInboundReversalRepository inboundReversalsRepository)
    {
        inboundReversalRepository = inboundReversalsRepository;
    }

    public Task<PaginatedList<ReversibleInboundDto>> Handle(GetReversibleInboundsQuery request, CancellationToken cancellationToken)
    {
        var result = inboundReversalRepository.GetReversibleInbounds(
            request.Pagination,
            request.WhsCode,
            request.InJobNo,
            request.NewerThan
        );
        return Task.FromResult(result);
    }
}

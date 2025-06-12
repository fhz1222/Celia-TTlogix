using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InboundReversalItems.Queries.GetReversibleInboundItems;

public class GetReversibleInboundItemsQuery : IRequest<IEnumerable<ReversibleInboundItemDto>>
{
    public string InJobNo { get; set; } = null!;
    public GetReversibleInboundItemsDtoFilter Filter { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetInboundItemsQueryHandler : IRequestHandler<GetReversibleInboundItemsQuery, IEnumerable<ReversibleInboundItemDto>>
{
    private readonly IInboundReversalRepository inboundReversalRepository;

    public GetInboundItemsQueryHandler(IInboundReversalRepository inboundReversalsRepository)
    {
        inboundReversalRepository = inboundReversalsRepository;
    }

    public Task<IEnumerable<ReversibleInboundItemDto>> Handle(GetReversibleInboundItemsQuery request, CancellationToken cancellationToken)
    {
        var result = inboundReversalRepository.GetReversibleInboundItems(request.InJobNo, request.Filter, request.OrderByExpression, request.OrderByDescending);
        return Task.FromResult(result);
    }
}

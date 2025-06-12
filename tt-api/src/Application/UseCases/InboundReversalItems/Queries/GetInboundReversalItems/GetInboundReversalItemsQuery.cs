using Application.Common.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InboundReversalItems.Queries.GetInboundReversalItems;

public class GetInboundReversalItemsQuery : IRequest<IEnumerable<InboundReversalItemDto>>
{
    public string JobNo { get; set; } = null!;
    public GetInboundReversalItemsDtoFilter Filter { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetInboundReversalsQueryHandler : IRequestHandler<GetInboundReversalItemsQuery, IEnumerable<InboundReversalItemDto>>
{
    private readonly IInboundReversalRepository inboundReversalRepository;

    public GetInboundReversalsQueryHandler(IInboundReversalRepository inboundReversalsRepository)
    {
        inboundReversalRepository = inboundReversalsRepository;
    }

    public Task<IEnumerable<InboundReversalItemDto>> Handle(GetInboundReversalItemsQuery request, CancellationToken cancellationToken)
    {
        var result = inboundReversalRepository.GetInboundReversalItems(request.JobNo, request.Filter, request.OrderByExpression, request.OrderByDescending);
        return Task.FromResult(result);
    }
}

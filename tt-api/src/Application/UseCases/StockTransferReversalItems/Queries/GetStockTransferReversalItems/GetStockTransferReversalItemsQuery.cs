using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.StockTransferReversalItems.Queries.GetStockTransferReversalItems;

public class GetStockTransferReversalItemsQuery : IRequest<IEnumerable<StockTransferReversalItemDto>>
{
    public string JobNo { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetStockTransferReversalsQueryHandler : IRequestHandler<GetStockTransferReversalItemsQuery, IEnumerable<StockTransferReversalItemDto>>
{
    private readonly IStockTransferReversalRepository StockTransferReversalRepository;

    public GetStockTransferReversalsQueryHandler(IStockTransferReversalRepository StockTransferReversalsRepository)
    {
        StockTransferReversalRepository = StockTransferReversalsRepository;
    }

    public Task<IEnumerable<StockTransferReversalItemDto>> Handle(GetStockTransferReversalItemsQuery request, CancellationToken cancellationToken)
    {
        var result = StockTransferReversalRepository.GetStockTransferReversalItems(request.JobNo, request.OrderByExpression, request.OrderByDescending);
        return Task.FromResult(result.AsEnumerable());
    }
}

using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.StockTransferReversalItems.Queries.GetReversibleStockTransferItems;

public class GetReversibleStockTransferItemsQuery : IRequest<IEnumerable<ReversibleStockTransferItemDto>>
{
    public string StfJobNo { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetStockTransferItemsQueryHandler : IRequestHandler<GetReversibleStockTransferItemsQuery, IEnumerable<ReversibleStockTransferItemDto>>
{
    private readonly IStockTransferReversalRepository StockTransferReversalRepository;

    public GetStockTransferItemsQueryHandler(IStockTransferReversalRepository StockTransferReversalsRepository)
    {
        StockTransferReversalRepository = StockTransferReversalsRepository;
    }

    public Task<IEnumerable<ReversibleStockTransferItemDto>> Handle(GetReversibleStockTransferItemsQuery request, CancellationToken cancellationToken)
    {
        var result = StockTransferReversalRepository.GetReversibleStockTransferItems(request.StfJobNo, request.OrderByExpression, request.OrderByDescending);
        return Task.FromResult(result.AsEnumerable());
    }
}

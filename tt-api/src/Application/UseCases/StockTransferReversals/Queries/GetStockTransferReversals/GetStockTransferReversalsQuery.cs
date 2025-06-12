using Application.Common.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.StockTransferReversals.Queries.GetStockTransferReversals;

public class GetStockTransferReversalsQuery : IRequest<PaginatedList<StockTransferReversalDto>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public GetStockTransferReversalsDtoFilter Filter { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetStockTransferReversalsQueryHandler : IRequestHandler<GetStockTransferReversalsQuery, PaginatedList<StockTransferReversalDto>>
{
    private readonly IStockTransferReversalRepository stockTransferReversalRepository;

    public GetStockTransferReversalsQueryHandler(IStockTransferReversalRepository stockTransferReversalRepository)
    {
        this.stockTransferReversalRepository = stockTransferReversalRepository;
    }

    public async Task<PaginatedList<StockTransferReversalDto>> Handle(GetStockTransferReversalsQuery request, CancellationToken cancellationToken)
    {
        var result = stockTransferReversalRepository.GetStockTransferReversals(request.Pagination, request.Filter, request.OrderByExpression, request.OrderByDescending);
        return await Task.FromResult(result);
    }
}

using Application.Common.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.StockTransferReversals.Queries.GetReversibleStockTransfers;

public class GetReversibleStockTransfersQuery : IRequest<PaginatedList<ReversibleStockTransferDto>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string? StfJobNo { get; set; } = null!;
    public DateTime? NewerThan { get; set; }
}

public class GetReversibleStockTransfersQueryHandler : IRequestHandler<GetReversibleStockTransfersQuery, PaginatedList<ReversibleStockTransferDto>>
{
    private readonly IStockTransferReversalRepository stockTransferReversalRepository;

    public GetReversibleStockTransfersQueryHandler(IStockTransferReversalRepository stockTransferReversalRepository)
    {
        this.stockTransferReversalRepository = stockTransferReversalRepository;
    }

    public async Task<PaginatedList<ReversibleStockTransferDto>> Handle(GetReversibleStockTransfersQuery request, CancellationToken cancellationToken)
    {
        var result = stockTransferReversalRepository.GetReversibleStockTransfers(
            request.Pagination,
            request.WhsCode,
            request.StfJobNo,
            request.NewerThan
        );
        return await Task.FromResult(result);
    }
}

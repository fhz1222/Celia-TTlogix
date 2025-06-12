using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.StockTransfer.Queries.GetStockTransferPalletsInILog;

public class GetStockTransferPalletsInILogQuery : IRequest<IEnumerable<string>>
{
    public string JobNo { get; set; }
}

public class GetStockTransferPalletsInILogQueryHandler : IRequestHandler<GetStockTransferPalletsInILogQuery, IEnumerable<string>>
{
    private readonly IStockTransferRepository stockTransfers;
    private readonly ILocationRepository locations;

    public GetStockTransferPalletsInILogQueryHandler(IStockTransferRepository stockTransfers, ILocationRepository locations)
    {
        this.stockTransfers = stockTransfers;
        this.locations = locations;
    }

    public Task<IEnumerable<string>> Handle(GetStockTransferPalletsInILogQuery request, CancellationToken cancellationToken)
    {
        var iLogStorageLocationCategory = locations.GetILogStorageLocationCategoryId();
        var pallets = stockTransfers.GetStockTransferPalletsOnLocationCategory(request.JobNo, iLogStorageLocationCategory);
        return Task.FromResult(pallets);
    }
}

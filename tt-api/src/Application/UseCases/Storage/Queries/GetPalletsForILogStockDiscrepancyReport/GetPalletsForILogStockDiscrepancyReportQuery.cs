using Application.Interfaces.Repositories;
using Application.UseCases.StorageDetails;
using MediatR;

namespace Application.UseCases.Storage.Queries.GetPalletsForILogStockDiscrepancyReport;

public class GetPalletsForILogStockDiscrepancyReportQuery : IRequest<IEnumerable<StockDiscrepancyReportPalletDto>>
{
    public StockDiscrepancyReportPalletDtoFilter Filter { get; set; } = null!;
}

public class GetPalletsForILogStockDiscrepancyReportQueryHandler : IRequestHandler<GetPalletsForILogStockDiscrepancyReportQuery, IEnumerable<StockDiscrepancyReportPalletDto>>
{
    private readonly IStorageDetailRepository storageDetailRepository;
    private readonly ILocationRepository locationRepository;

    public GetPalletsForILogStockDiscrepancyReportQueryHandler(IStorageDetailRepository storageDetailRepository, ILocationRepository locationRepository)
    {
        this.storageDetailRepository = storageDetailRepository;
        this.locationRepository = locationRepository;
    }

    public async Task<IEnumerable<StockDiscrepancyReportPalletDto>> Handle(GetPalletsForILogStockDiscrepancyReportQuery request, CancellationToken cancellationToken)
    {
        var iLogStorageCategoryId = locationRepository.GetILogStorageLocationCategoryId();
        var results = storageDetailRepository.GetPalletsForILogStockDiscrepancyReport(request.Filter, iLogStorageCategoryId);
        return await Task.FromResult(results);
    }
}

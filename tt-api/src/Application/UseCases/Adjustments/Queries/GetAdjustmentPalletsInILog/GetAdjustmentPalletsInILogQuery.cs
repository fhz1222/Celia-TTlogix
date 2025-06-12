using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Adjustments.Queries.GetAdjustmentPalletsInILog;

public class GetAdjustmentPalletsInILogQuery : IRequest<IEnumerable<AdjustmentPalletDto>>
{
    public string JobNo { get; set; }
}

public class GetQuerantinePalletsInILogQueryHandler : IRequestHandler<GetAdjustmentPalletsInILogQuery, IEnumerable<AdjustmentPalletDto>>
{
    private readonly IAdjustmentItemRepository adjustments;
    private readonly ILocationRepository locations;

    public GetQuerantinePalletsInILogQueryHandler(IAdjustmentItemRepository adjustments, ILocationRepository locations)
    {
        this.adjustments = adjustments;
        this.locations = locations;
    }

    public Task<IEnumerable<AdjustmentPalletDto>> Handle(GetAdjustmentPalletsInILogQuery request, CancellationToken cancellationToken)
    {
        var iLogStorageLocationCategory = locations.GetILogStorageLocationCategoryId();
        var pallets = adjustments.GetAdjustmentPalletsOnLocationCategory(request.JobNo, iLogStorageLocationCategory);
        return Task.FromResult(pallets);
    }
}

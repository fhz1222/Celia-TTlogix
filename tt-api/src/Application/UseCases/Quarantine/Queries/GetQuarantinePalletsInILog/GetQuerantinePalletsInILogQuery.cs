using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Quarantine.Queries.GetQuarantinePalletsInILog;

public class GetQuerantinePalletsInILogQuery : IRequest<IEnumerable<QuarantinePalletDto>>
{
    public string JobNo { get; set; }
}

public class GetQuerantinePalletsInILogQueryHandler : IRequestHandler<GetQuerantinePalletsInILogQuery, IEnumerable<QuarantinePalletDto>>
{
    private readonly IQuarantineRepository quarantines;
    private readonly ILocationRepository locations;

    public GetQuerantinePalletsInILogQueryHandler(IQuarantineRepository quarantines, ILocationRepository locations)
    {
        this.quarantines = quarantines;
        this.locations = locations;
    }

    public Task<IEnumerable<QuarantinePalletDto>> Handle(GetQuerantinePalletsInILogQuery request, CancellationToken cancellationToken)
    {
        var iLogStorageLocationCategory = locations.GetILogStorageLocationCategoryId();
        var pallets = quarantines.GetQuarantinePalletsOnLocationCategory(request.JobNo, iLogStorageLocationCategory);
        return Task.FromResult(pallets);
    }
}

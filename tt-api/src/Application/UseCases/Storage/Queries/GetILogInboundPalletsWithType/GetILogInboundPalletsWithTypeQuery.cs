using Application.Interfaces.Repositories;
using Application.UseCases.StorageDetails;
using MediatR;

namespace Application.UseCases.Storage.Queries.GetILogInboundPalletsWithTypeQuery;

public class GetILogInboundPalletsWithTypeQuery : IRequest<IEnumerable<ILogInboundPalletWithTypeDto>>
{
    public ILogInboundPalletsWithTypeDtoFilter Filter { get; set; } = null!;
}

public class GetILogInboundPalletsWithTypeQueryHandler : IRequestHandler<GetILogInboundPalletsWithTypeQuery, IEnumerable<ILogInboundPalletWithTypeDto>>
{
    private readonly IStorageDetailRepository storageDetailRepository;
    private readonly ILocationRepository locationRepository;

    public GetILogInboundPalletsWithTypeQueryHandler(IStorageDetailRepository storageDetailRepository, ILocationRepository locationRepository)
    {
        this.storageDetailRepository = storageDetailRepository;
        this.locationRepository = locationRepository;
    }

    public async Task<IEnumerable<ILogInboundPalletWithTypeDto>> Handle(GetILogInboundPalletsWithTypeQuery request, CancellationToken cancellationToken)
    {
        var iLogInboundCategoryId = locationRepository.GetILogInboundLocationCategoryId();
        var results = storageDetailRepository.GetILogInboundPalletsWithType(request.Filter, iLogInboundCategoryId);
        return await Task.FromResult(results);
    }
}

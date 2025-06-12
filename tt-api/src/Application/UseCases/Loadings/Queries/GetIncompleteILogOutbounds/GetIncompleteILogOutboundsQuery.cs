using Application.Interfaces.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Loadings.Queries.GetIncompleteILogOutbounds;

public class GetIncompleteILogOutboundsQuery : IRequest<List<string>>
{
    public string LoadingJobNo { get; set; }
}

public class GetIncompleteILogOutboundsQueryHandler : IRequestHandler<GetIncompleteILogOutboundsQuery, List<string>>
{
    IOutboundRepository outboundRepository { get; set; }
    IILogPickingRequestRepository iLogPickingRequestRepository { get; set; }

    public GetIncompleteILogOutboundsQueryHandler(IOutboundRepository outboundRepository, IILogPickingRequestRepository iLogPickingRequestRepository)
    {
        this.outboundRepository = outboundRepository;
        this.iLogPickingRequestRepository = iLogPickingRequestRepository;
    }

    public Task<List<string>> Handle(GetIncompleteILogOutboundsQuery request, CancellationToken cancellationToken)
    {
        //get new and partial picked outbounds from loading

        var jobNos = outboundRepository.GetOutboundsOnLoading(request.LoadingJobNo)
            .Where(x => x.Status == OutboundStatus.New || x.Status == OutboundStatus.PartialPicked)
            .Select(x => x.JobNo)
            .ToList();
        //get all picking request revisions for the outbounds
        var pickingRequestRevisions = iLogPickingRequestRepository.GetPickingRequestRevisionsNoTracking(jobNos);
        //get only those outbounds that do not have any open revision
        var res = pickingRequestRevisions.GroupBy(x => x.OutboundJobNo).Where(x => x.All(y => y.IsClosed)).Select(x => x.Key).ToList();
        return Task.FromResult(res);
    }
}
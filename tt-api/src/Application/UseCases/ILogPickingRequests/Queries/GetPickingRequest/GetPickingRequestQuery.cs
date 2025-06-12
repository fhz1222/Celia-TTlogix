using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogPickingRequests.Queries.GetPickingRequest;

public class GetPickingRequestQuery : IRequest<PickingRequestDto>
{
    public string PickingRequestId { get; set; }
    public int Revision { get; set; }
}

public class GetPickingRequestQueryHandler : IRequestHandler<GetPickingRequestQuery, PickingRequestDto>
{
    private readonly IRepository repository;

    public GetPickingRequestQueryHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public Task<PickingRequestDto> Handle(GetPickingRequestQuery r, CancellationToken cancellationToken)
    {
        var request = repository.ILogPickingRequests.GetRevision(r.PickingRequestId, r.Revision)
            ?? throw new ApplicationError($"Picking Request {r.PickingRequestId} rev {r.Revision} does not exist.");

        var outbound = repository.Picking.GetOutbound(request.OutboundJobNo)
            ?? throw new ApplicationError($"Outbound {request.OutboundJobNo} does not exist.");

        var loading = repository.Outbounds.GetLoadingByOutboundNoTracking(outbound.JobNo);

        var requestLines = repository.ILogPickingRequests.GetItemsWithUnloadingPoint(request.PickingRequestId, request.Revision);

        var dto = new PickingRequestDto()
        {
            PickingRequestId = request.PickingRequestId,
            PickingRequestRevision = request.Revision,
            OutboundJobNo = request.OutboundJobNo,
            OrderNo = outbound.OrderNo,
            OutboundRemarks = outbound.Remarks,
            LoadingETA = loading?.ETA,
            LoadingETD = loading?.ETD,
            CustomerCode = outbound.CustomerCode,
            WarehouseCode = outbound.Whs,
            Lines = requestLines.ToArray()
        };

        return Task.FromResult(dto);
    }
}

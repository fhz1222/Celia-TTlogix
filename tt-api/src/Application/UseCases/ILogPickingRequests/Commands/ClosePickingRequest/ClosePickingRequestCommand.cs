using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.ILogPickingRequests.Commands.ClosePickingRequest;

public class ClosePickingRequestCommand : IRequest<ClosePickingRequestResponseDto>
{
    public string PickingRequest { get; set; } = null!;
}

public class ClosePickingRequestCommandHandler : IRequestHandler<ClosePickingRequestCommand, ClosePickingRequestResponseDto>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTime;

    public ClosePickingRequestCommandHandler(IRepository repository, IDateTime dateTime)
    {
        this.repository = repository;
        this.dateTime = dateTime;
    }

    public async Task<ClosePickingRequestResponseDto> Handle(ClosePickingRequestCommand request, CancellationToken cancellationToken)
    {
        var pickingReq = repository.ILogPickingRequests.GetLastRevision(request.PickingRequest)
            ?? throw new ApplicationError($"Picking request {request.PickingRequest} does not exist.");

        if (pickingReq.IsClosed)
        {
            throw new ApplicationError($"Picking request {pickingReq.PickingRequestId} rev {pickingReq.Revision} is already closed.");
        }

        pickingReq.Close(dateTime.Now);
        repository.ILogPickingRequests.Update(pickingReq);
        await repository.SaveChangesAsync(cancellationToken);

        var outbound = repository.Picking.GetOutbound(pickingReq.OutboundJobNo);
        var response = new ClosePickingRequestResponseDto(outbound!.JobNo, outbound.Status == OutboundStatus.Picked);
        return response;
    }
}

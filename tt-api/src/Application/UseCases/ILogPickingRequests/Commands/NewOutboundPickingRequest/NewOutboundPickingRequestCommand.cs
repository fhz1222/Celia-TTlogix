using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.ILogPickingRequests.Commands.NewOutboundPickingRequest;

public class NewOutboundPickingRequestCommand : IRequest<CreatedOutboundPickingRequestDto>
{
    public string UserCode { get; set; } = null!;
    public string OutboundJobNo { get; set; } = null!;
}

public class NewOutboundPickingRequestCommandHandler : IRequestHandler<NewOutboundPickingRequestCommand, CreatedOutboundPickingRequestDto>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTime;
    private readonly IILogConnectGateway iLogConnectGateway;

    public NewOutboundPickingRequestCommandHandler(IRepository repository, IDateTime dateTime, IILogConnectGateway iLogConnectGateway)
    {
        this.repository = repository;
        this.dateTime = dateTime;
        this.iLogConnectGateway = iLogConnectGateway;
    }

    public async Task<CreatedOutboundPickingRequestDto> Handle(NewOutboundPickingRequestCommand r, CancellationToken cancellationToken)
    {
        if (!repository.Utils.CheckIfUserCodeExists(r.UserCode))
            throw new UnknownUserCodeException();

        var jobNo = r.OutboundJobNo;
        var pickingItems = GetILogStoragePickingListItems(jobNo);
        if (pickingItems.None())
        {
            throw new FailedToCreateILogPickingRequestException("Nothing to send to iLog (no pallets on iLog Storage locations).");
        }

        PickingRequestRevision? newRequest;
        try
        {
            repository.BeginTransaction();

            var openPickingRequest = repository.ILogPickingRequests.GetOpenRequest(jobNo);
            if (openPickingRequest == null)
            {
                newRequest = CreateNewRequest(jobNo, pickingItems, r.UserCode);
            }
            else
            {
                var isInProgress = await iLogConnectGateway.IsPickingRequestStarted(openPickingRequest.PickingRequestId, cancellationToken);
                if (isInProgress)
                {
                    throw new FailedToCreateILogPickingRequestException($"There is ongoing picking request for this outbound (request: {openPickingRequest.PickingRequestId}).");
                }
                else
                {
                    newRequest = CreateNewRevisionOfRequest(openPickingRequest, pickingItems, r.UserCode);
                }
            }

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
        }
        catch
        {
            repository.RollbackTransaction();
            throw;
        }

        if (newRequest is { })
        {
            iLogConnectGateway.PickingRequestCreated(newRequest.PickingRequestId, newRequest.Revision);
            //todo: mark that picking request was successfuly integrated with iLog?
        }

        return new CreatedOutboundPickingRequestDto
        {
            PickingRequestId = newRequest.PickingRequestId,
            Revision = newRequest.Revision,
            PalletsCount = pickingItems.Count
        };
    }

    private PickingRequestRevision CreateNewRequest(string jobNo, List<PickingListItem> pickingItems, string userCode)
    {
        var outbound = repository.Picking.GetOutbound(jobNo) ?? throw new ApplicationError($"Outbound {jobNo} does not exist.");

        // todo: instead create items and save them to the database into 3 tables
        var revision = repository.ILogPickingRequests.AddNewRequest(outbound, userCode, dateTime.Now);
        var items = pickingItems.Select(x => new PickingRequestRevisionItem(revision.PickingRequestId, x));
        repository.ILogPickingRequests.AddItems(revision, items);

        return revision;
    }

    private PickingRequestRevision CreateNewRevisionOfRequest(PickingRequestRevision previousRevision, List<PickingListItem> pickingItems, string userCode)
    {
        previousRevision.Close(dateTime.Now);
        repository.ILogPickingRequests.Update(previousRevision);

        var revision = new PickingRequestRevision(previousRevision, userCode, dateTime.Now);
        repository.ILogPickingRequests.AddNewRevision(revision);
        var items = pickingItems.Select(x => new PickingRequestRevisionItem(revision.PickingRequestId, x));
        repository.ILogPickingRequests.AddItems(revision, items);

        return revision;
    }

    private List<PickingListItem> GetILogStoragePickingListItems(string outboundJobNo)
    {
        var iLogStorageCategory = repository.Locations.GetILogStorageLocationCategoryId();
        var items = repository.Picking.GetPickingItemsOnILogStorage(outboundJobNo, iLogStorageCategory)
            .Where(x => x.IsNotPicked)
            .ToList();
        return items;
    }
}

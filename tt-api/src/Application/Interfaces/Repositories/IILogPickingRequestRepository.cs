using Application.UseCases.ILogPickingRequests.Queries.GetPickingRequest;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IILogPickingRequestRepository
{
    PickingRequestRevision AddNewRequest(Outbound outbound, string userCode, DateTime createdDate);
    void AddNewRevision(PickingRequestRevision oldPickingRequest);
    PickingRequestRevision? GetOpenRequest(string outboundJobNo);
    void Update(PickingRequestRevision pickingRequest);
    void AddItems(PickingRequestRevision pickingRequest, IEnumerable<PickingRequestRevisionItem> items);
    PickingRequestRevision? GetLastRevision(string requestId);
    List<PickingRequestRevision> GetPickingRequestRevisionsNoTracking(List<string> outboundJobNos);
    PickingRequestRevision? GetRevision(string requestId, int revision);
    List<PickingRequestLineDto> GetItemsWithUnloadingPoint(string requestId, int revision);
    void CancelAllOpenRequests();
}

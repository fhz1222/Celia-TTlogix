namespace Application.UseCases.ILogPickingRequests.Queries.GetPickingRequest;

public class PickingRequestDto
{
    public string PickingRequestId { get; set; }
    public int PickingRequestRevision { get; set; }
    public string OutboundJobNo { get; set; }
    public string OrderNo { get; set; }
    public string OutboundRemarks { get; set; }
    public DateTime? LoadingETA { get; set; }
    public DateTime? LoadingETD { get; set; }
    public string CustomerCode { get; set; }
    public string WarehouseCode { get; set; }
    public PickingRequestLineDto[] Lines { get; set; }
}

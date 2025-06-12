namespace Application.UseCases.ILogPickingRequests;

public class CreatedOutboundPickingRequestDto
{
    public string PickingRequestId { get; set; } = null!;
    public int Revision { get; set; }
    public int PalletsCount { get; set; }
}

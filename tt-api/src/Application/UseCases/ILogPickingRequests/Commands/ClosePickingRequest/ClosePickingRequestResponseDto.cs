namespace Application.UseCases.ILogPickingRequests.Commands.ClosePickingRequest;

public class ClosePickingRequestResponseDto
{
    public string OutboundJobNo { get; set; }
    public bool IsOutboundFullyPicked { get; set; }

    public ClosePickingRequestResponseDto(string jobNo, bool isOutboundFullyPicked)
    {
        OutboundJobNo = jobNo;
        IsOutboundFullyPicked = isOutboundFullyPicked;
    }
}

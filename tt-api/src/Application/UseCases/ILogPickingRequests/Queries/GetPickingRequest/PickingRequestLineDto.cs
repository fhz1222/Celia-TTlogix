namespace Application.UseCases.ILogPickingRequests.Queries.GetPickingRequest;

public class PickingRequestLineDto
{
    public int LineNo { get; set; }
    public string ProductCode { get; set; }
    public string SupplierId { get; set; }
    public int Qty { get; set; }
    public string? PalletId { get; set; }
    public string? ProductUnloadingPoint { get; set; }
}

namespace Application.UseCases.InboundReversals;

public class ReversibleInboundDto
{
    public string JobNo { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
}

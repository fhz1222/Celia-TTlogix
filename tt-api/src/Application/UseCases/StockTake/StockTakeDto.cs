using Domain.ValueObjects;

namespace Application.UseCases.StockTake;

public class StockTakeDto
{
    public string JobNo { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string LocationCode { get; set; } = null!;
    public string CreatedDate { get; set; } = null!;
    public JobStatus Status { get; set; } = null!;
    public string Remark { get; set; } = null!;
}

using Application.Common.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Adjustments.Queries.GetAdjustmentsQuery;

public class AdjustmentFilter
{
    public string? CustomerCode { get; set; }
    public string WhsCode { get; set; } = null!;
    public string? JobNo { get; set; }
    public InventoryAdjustmentJobType? JobType { get; set; }
    public DtoFilterDateTimeRange? CreatedDate { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Reason { get; set; }
    public AdjustmentFilterStatus Status { get; set; }
}
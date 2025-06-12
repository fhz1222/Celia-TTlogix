using Application.Common.Enums;

namespace Application.UseCases.Decants.Queries.GetDecants;

public class DecantDtoFilter
{
    public string? CustomerCode { get; set; }
    public string WhsCode { get; set; } = null!;
    public string? JobNo { get; set; }
    public DtoFilterDateTimeRange? CreatedDate { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Remark { get; set; }
    public DecantFilterStatus Status { get; set; } = DecantFilterStatus.Outstanding;
}
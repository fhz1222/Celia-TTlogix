namespace Application.UseCases.Registration.Queries.GetAreaList;

public class GetAreaListDtoFilter
{
    public string? Code { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? Type { get; set; }
    public string? WhsCode { get; set; }
    public int? Status { get; set; }
}

namespace Application.UseCases.Registration.Queries.GetLabelPrinterList;

public class GetLabelPrinterListDtoFilter
{
    public string? Ip { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? Description { get; set; }
    public int? Type { get; set; }
}

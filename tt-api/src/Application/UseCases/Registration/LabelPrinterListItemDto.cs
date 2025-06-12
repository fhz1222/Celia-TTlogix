using Domain.ValueObjects;

namespace Application.UseCases.Registration;

public class LabelPrinterListItemDto
{
    public string Ip { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Type { get; set; }
}

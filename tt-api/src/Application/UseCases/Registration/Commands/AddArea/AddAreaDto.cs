namespace Application.UseCases.Registration.Commands.AddArea;

public class AddAreaDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public decimal Capacity { get; set; }
}

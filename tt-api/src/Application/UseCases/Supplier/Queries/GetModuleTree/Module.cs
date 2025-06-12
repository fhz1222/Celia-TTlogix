namespace Application.UseCases.Supplier.Queries.GetModuleTree;

public class Module
{
    public string Code { get; set; } = default!;
    public string ParentCode { get; set; } = default!;
    public string ModuleName { get; set; } = default!;
    public string NavigateUrl { get; set; } = default!;
}

namespace Application.UseCases.Supplier.Queries.GetModuleTree;

public class ModuleTreeItemDto
{
    public string ModuleName { get; set; } = default!;
    public string NavigateUrl { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string ParentCode { get; set; } = default!;
    public List<ModuleTreeItemDto> Children { get; set; } = new();
}
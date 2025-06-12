using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Supplier.Queries.GetModuleTree;

public class GetModuleTreeQuery : IRequest<List<ModuleTreeItemDto>>
{
    public string LoginId { get; set; } = default!;
}

public class GetModuleTreeQueryHandler : IRequestHandler<GetModuleTreeQuery, List<ModuleTreeItemDto>>
{
    private readonly ISupplierRepository repository;

    public GetModuleTreeQueryHandler(ISupplierRepository repository) => this.repository = repository;

    public Task<List<ModuleTreeItemDto>> Handle(GetModuleTreeQuery r, CancellationToken cancellationToken)
    {
        var modules = repository.GetModules(r.LoginId);
        var moduleTree = GetModuleTreeSubitems(modules, string.Empty).ToList();
        return Task.FromResult(moduleTree);
    }

    private IEnumerable<ModuleTreeItemDto> GetModuleTreeSubitems(List<Module> modules, string parentCode)
    {
        var submodules = modules.Where(m => (m.ParentCode?.Trim() ?? string.Empty) == parentCode).ToList();
        foreach (var sub in submodules)
        {
            yield return new ModuleTreeItemDto
            {
                Code = sub.Code,
                ParentCode = sub.ParentCode,
                ModuleName = sub.ModuleName,
                NavigateUrl = sub.NavigateUrl,
                Children = GetModuleTreeSubitems(modules, sub.Code).ToList()
            };
        }
    }
}

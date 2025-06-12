using Application.UseCases.Supplier;
using Application.UseCases.Supplier.Queries.GetModuleTree;
using Application.UseCases.Supplier.Queries.GetSuppliers;

namespace Application.Interfaces.Repositories
{
    public interface ISupplierRepository
    {
        List<Module> GetModules(string loginId);
        IEnumerable<SupplierDto> GetSuppliers(SupplierDtoFilter filter);
    }
}
using Application.Interfaces.Repositories;
using Application.UseCases.Supplier;
using Application.UseCases.Supplier.Queries.GetModuleTree;
using Application.UseCases.Supplier.Queries.GetSuppliers;
using Persistence.Entities;
using Persistence.PetaPoco;
using PetaPoco;
using System.Linq.Expressions;
using IMapper = AutoMapper.IMapper;

namespace Persistence.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext context;
    private readonly Database pocoContext;
    private readonly IMapper mapper;

    public SupplierRepository(AppDbContext context, IPPDbContextFactory factory, IMapper mapper)
    {
        this.context = context;
        this.pocoContext = factory.GetInstance();
        this.mapper = mapper;
    }

    public IEnumerable<SupplierDto> GetSuppliers(SupplierDtoFilter filter)
    {
        Expression<Func<SupplierMaster, bool>> factoryIdsFilter =
            filter.FactoryIds == null ? (sm => true) : (sm => filter.FactoryIds!.Contains(sm.FactoryId));

        var suppliers = context.SupplierMaster
            .Where(factoryIdsFilter);

        return mapper.Map<IEnumerable<SupplierDto>>(suppliers);
    }

    public List<Module> GetModules(string loginId)
    {
        var displayTree = pocoContext.Query<string>(@"
SELECT DisplayTree FROM VMISUser u JOIN Role r ON u.Roles = r.RoleName WHERE u.LoginID = @loginId", new { loginId })?.FirstOrDefault();
        var modules = pocoContext.Query<Module>(@$"SELECT * FROM Module WHERE Code IN ({displayTree}) ORDER BY CONVERT(FLOAT, code)")
            .ToList();
        return modules;
    }
}

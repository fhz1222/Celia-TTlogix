using Application.Interfaces.Repositories;
using Application.UseCases.Product;
using Application.UseCases.Product.Queries;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;
using Persistence.PetaPoco;
using Persistence.PetaPoco.Models;
using PetaPoco;
using PetaPoco.SqlKata;
using SqlKata;
using System.Linq.Expressions;

namespace Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext context;
    private readonly Database ppContext;

    public ProductRepository(AppDbContext context, IPPDbContextFactory factory)
    {
        this.context = context;
        ppContext = factory.GetInstance();
    }

    public IEnumerable<ProductWithUOMDto> GetProductsWithUOM(ProductWithUOMDtoFilter filter)
    {
        Expression<Func<TtPartMaster, bool>> customerCodesFilter =
            filter.CustomerCodes == null ? (pm => true) : (pm => filter.CustomerCodes!.Contains(pm.CustomerCode));

        var results = context.TtPartMasters
            .Where(customerCodesFilter)
            .Join(context.TtUOM, pm => pm.Uom, uom => uom.Code, (pm, uom) => new ProductWithUOMDto
            {
                CustomerCode = pm.CustomerCode,
                SupplierId = pm.SupplierId,
                ProductCode = pm.ProductCode1,
                Description = pm.Description,
                UomName = uom.Name,
                LengthInternal = pm.LengthTt,
                WidthInternal = pm.WidthTt,
                HeightInternal = pm.HeightTt,
                NetWeightInternal = pm.NetWeightTt,
                GrossWeightInternal = pm.GrossWeightTt,
                Status = pm.Status,
                IsCpart = pm.IsCpart,
                IsMixed = pm.IsMixed,
                FloorStackability = pm.FloorStackability,
                TruckStackability = pm.TruckStackability
            });

        return results;
    }

    public bool? IsCPartProduct(string customer, string supplier, string productCode)
    {
        var query = new Query(TT_PartMaster.SqlTableName)
            .Where("CustomerCode", customer)
            .Where("SupplierID", supplier)
            .Where("ProductCode1", productCode)
            .Select("IsCPart")
            .ToSql();
        var cPart = ppContext.SingleOrDefault<int>(query);
        var isCPart = cPart == 1;
        return isCPart;
    }
}

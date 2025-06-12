using Application.UseCases.Product;
using Application.UseCases.Product.Queries;

namespace Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<ProductWithUOMDto> GetProductsWithUOM(ProductWithUOMDtoFilter filter);
        bool? IsCPartProduct(string customer, string supplier, string productCode);
    }
}
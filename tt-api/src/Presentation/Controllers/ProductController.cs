using Application.UseCases.Product;
using Application.UseCases.Product.Queries;
using Application.UseCases.Product.Queries.GetProductWithUOM;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// ProductController provides method to get the product list 
/// </summary>
public partial class ProductController : ApiControllerBase
{
    /// <summary>
    /// constructor
    /// </summary>
    public ProductController()
    {
    }

    /// <summary>
    /// Gets Products with UOM name included (filtered by customer codes)
    /// </summary>
    [HttpGet]
    [Route("productWithUOM")]
    [Produces("application/json")]
    public async Task<IEnumerable<ProductWithUOMDto>> GetProductsWithUOM([FromQuery] ProductWithUOMDtoFilter filter)
    {
        return await Mediator.Send(new GetProductWithUOMQuery
        {
            filter = filter
        });
    }
}



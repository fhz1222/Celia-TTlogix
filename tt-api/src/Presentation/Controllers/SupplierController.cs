using Application.UseCases.Supplier;
using Application.UseCases.Supplier.Queries.GetModuleTree;
using Application.UseCases.Supplier.Queries.GetSuppliers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// SupplierController provides method to get the supplier list (filtered by factoryIds)
/// </summary>
public partial class SupplierController : ApiControllerBase
{
    private readonly IMapper mapper;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public SupplierController(IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets Suppliers
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public async Task<IEnumerable<SupplierDto>> GetSuppliers([FromQuery] SupplierDtoFilter filter)
    {
        return await Mediator.Send(new GetSuppliersQuery
        {
            filter = filter
        });
    }

    /// <summary>
    /// Gets list of modules available for VMI login
    /// </summary>
    /// <param name="loginId">Login ID</param>
    /// <returns>List of modules the login has access to</returns>
    [HttpGet("moduleTree/{loginId}")]
    public async Task<List<ModuleTreeItemDto>> GetModuleTree([FromRoute] string loginId)
        => await Mediator.Send(new GetModuleTreeQuery() { LoginId = loginId });
}



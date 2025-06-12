using Application.Common.Models;
using Application.UseCases.Inventory;
using Application.UseCases.Inventory.Queries.GetInventoryItems;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;

namespace Presentation.Controllers;
/// <summary>
/// InventoryControler provides method to get the storage detail list
/// </summary>
public partial class InventoryController : ApiControllerBase
{
    private readonly IMapper mapper;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public InventoryController( IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets inventory items filtered by specified filters
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public async Task<PaginatedList<InventoryItemDto>> GetInventoryItems([FromQuery] InventoryItemParameters filter, string whsCode)
    {
        var gridFilter = mapper.Map<InventoryItemDtoFilter>(filter);
        gridFilter.WhsCode = whsCode;
        return await Mediator.Send(new GetInventoryItemsQuery()
        {
            Pagination = filter.Pagination,
            Filter = gridFilter,
            OrderByExpression = filter.Sorting?.By,
            OrderByDescending = filter.Sorting?.Descending ?? false
        });
    }
}



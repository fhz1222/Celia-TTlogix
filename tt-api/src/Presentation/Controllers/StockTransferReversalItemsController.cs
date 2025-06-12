using Application.UseCases.StockTransferReversalItems.Queries.GetStockTransferReversalItems;
using Application.UseCases.StockTransferReversalItems;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Presentation.Common;
using Presentation.Configuration;
using Application.UseCases.StockTransferReversalItems.Commands.DeleteStockTransferReversalItemCommand;
using Application.UseCases.StockTransferReversals;
using Application.UseCases.StockTransferReversalItems.Commands.AddNewStockTransferReversalItemCommand;
using Application.UseCases.StockTransferReversalItems.Queries.GetReversibleStockTransferItems;
using Application.Common.Models;

namespace Presentation.Controllers;

/// <summary>
/// StockTransferReversalItemsController provides methods related to Stock Transfer reversal items
/// </summary>
public partial class StockTransferReversalItemsController : ApiControllerBase
{
    private readonly IMapper mapper;
    private readonly IFeatureManager manager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="manager"></param>
    public StockTransferReversalItemsController(IMapper mapper, IFeatureManager manager)
    {
        this.mapper = mapper;
        this.manager = manager;
    }

    [HttpGet("isActive")]
    public async Task<bool> IsActive()
        => await manager.IsEnabledAsync(FeatureFlags.StockTransferReversal);

    /// <summary>
    /// Gets stock transfer reversal items filtered by specified filters
    /// </summary>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpGet("getStockTransferReversalItems")]
    public async Task<IEnumerable<StockTransferReversalItemDto>> GetStockTransferReversalItems([FromQuery] StockTransferReversalItemsParameters parameters)
    {
        return await Mediator.Send(new GetStockTransferReversalItemsQuery()
        {
            JobNo = parameters.JobNo,
            OrderByExpression = parameters.Sorting?.By,
            OrderByDescending = parameters.Sorting?.Descending ?? false
        });
    }

    /// <summary>
    /// Gets reversible stock transfer items filtered by specified filters
    /// </summary>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpGet("getReversibleStockTransferItems")]
    public async Task<IEnumerable<ReversibleStockTransferItemDto>> GetReversibleStockTransferItems([FromQuery] ReversibleStockTransferItemsParameters parameters)
    {
        return await Mediator.Send(new GetReversibleStockTransferItemsQuery()
        {
            StfJobNo = parameters.StfJobNo,
            OrderByExpression = parameters.Sorting?.By,
            OrderByDescending = parameters.Sorting?.Descending ?? false
        });
    }

    /// <summary>
    /// Creates the new stock transfer reversal item object and returns it with values filled in
    /// </summary>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpPost("add")]
    public async Task<ActionResult> AddNewStockTransferReversalItem([FromBody] StockTransferReversalItemsAddParameters parameters, string userCode)
    {
        await Mediator.Send(new AddNewStockTransferReversalItemCommand()
        {
            PIDs = parameters.PIDs,
            JobNo = parameters.JobNo,
            UserCode = userCode,
        });
        return Ok();
    }

    /// <summary>
    /// Deletes the stock transfer reversal item
    /// </summary>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpDelete("delete")]
    public async Task<StockTransferReversalDetailsDto> DeleteStockTransferReversalItem(string jobNo, string PID)
    {
        return await Mediator.Send(new DeleteStockTransferReversalItemCommand()
        {
            JobNo = jobNo,
            PID = PID,
        });
    }
}



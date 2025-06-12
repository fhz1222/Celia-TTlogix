using Application.Common.Models;
using Application.UseCases.InboundReversalItems;
using Application.UseCases.InboundReversalItems.Commands.AddNewInboundReversalItemCommand;
using Application.UseCases.InboundReversalItems.Commands.DeleteInboundReversalItemCommand;
using Application.UseCases.InboundReversalItems.Queries.GetInboundReversalItems;
using Application.UseCases.InboundReversalItems.Queries.GetReversibleInboundItems;
using Application.UseCases.InboundReversals;
using Application.UseCases.InboundReversals.Queries.GetInboundReversalDetails;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Presentation.Common;
using Presentation.Configuration;
using Presentation.Utilities;

namespace Presentation.Controllers;

/// <summary>
/// InboundReversalItemsController provides methods related to inbound reversal items
/// </summary>
public partial class InboundReversalItemsController : ApiControllerBase
{
    private readonly IMapper mapper;
    private readonly IFeatureManager manager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="manager"></param>
    public InboundReversalItemsController(IMapper mapper, IFeatureManager manager)
    {
        this.mapper = mapper;
        this.manager = manager;
    }

    [HttpGet("isActive")]
    public async Task<bool> IsActive()
        => await manager.IsEnabledAsync(FeatureFlags.InboundReversal);

    /// <summary>
    /// Gets inbound reversal items filtered by specified filters
    /// </summary>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpGet]
    [Route("getInboundReversalItems")]
    [Produces("application/json")]
    public async Task<IEnumerable<InboundReversalItemDto>> GetInboundReversalItems([FromQuery] InboundReversalItemsParameters parameters)
    {
        var filter = mapper.Map<GetInboundReversalItemsDtoFilter>(parameters);
        return await Mediator.Send(new GetInboundReversalItemsQuery()
        {
            JobNo = parameters.JobNo,
            Filter = filter,
            OrderByExpression = parameters.Sorting?.By,
            OrderByDescending = parameters.Sorting?.Descending ?? false
        });
    }

    /// <summary>
    /// Gets reversible inbound items filtered by specified filters
    /// </summary>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpGet]
    [Route("getReversibleInboundItems")]
    [Produces("application/json")]
    public async Task<IEnumerable<ReversibleInboundItemDto>> GetReversibleInboundItems([FromQuery] ReversibleInboundItemsParameters parameters)
    {
        var filter = mapper.Map<GetReversibleInboundItemsDtoFilter>(parameters);
        return await Mediator.Send(new GetReversibleInboundItemsQuery()
        {
            InJobNo = parameters.InJobNo,
            Filter = filter,
            OrderByExpression = parameters.Sorting?.By,
            OrderByDescending = parameters.Sorting?.Descending ?? false
        });
    }

    /// <summary>
    /// Creates the new inbound reversal item object and returns it with values filled in
    /// </summary>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpPost]
    [Route("add")]
    [Produces("application/json")]
    public async Task<ActionResult> AddNewInboundReversalItem([FromBody] InboundReversalItemsAddParameters parameters, string userCode)
    {
        await Mediator.Send(new AddNewInboundReversalItemCommand()
        {
            PIDs = parameters.PIDs,
            JobNo = parameters.JobNo,
            UserCode = userCode,
        });
        return Ok();
    }

    /// <summary>
    /// Deletes the inbound reversal item
    /// </summary>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpDelete]
    [Route("delete")]
    [Produces("application/json")]
    public async Task<InboundReversalDetailsDto> DeleteInboundReversalItem(string jobNo, string PID)
    {
        await Mediator.Send(new DeleteInboundReversalItemCommand()
        {
            JobNo = jobNo,
            PID = PID,
        });
        return await Mediator.Send(new GetInboundReversalDetailsQuery
        {
            JobNo = jobNo
        });
    }
}



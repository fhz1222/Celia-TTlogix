using Application.Common.Models;
using Application.UseCases.InboundReversals;
using Application.UseCases.InboundReversals.Commands.AddNewInboundReversalCommand;
using Application.UseCases.InboundReversals.Commands.CancelInboundReversalCommand;
using Application.UseCases.InboundReversals.Commands.CompleteInboundReversalCommand;
using Application.UseCases.InboundReversals.Commands.UpdateInboundReversalCommand;
using Application.UseCases.InboundReversals.Queries.GetInboundReversalDetails;
using Application.UseCases.InboundReversals.Queries.GetInboundReversals;
using Application.UseCases.InboundReversals.Queries.GetReversibleInbounds;
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
/// InboundReversalController provides methods related to inbound reversals
/// </summary>
public partial class InboundReversalController : ApiControllerBase
{
    private readonly IMapper mapper;
    private readonly IFeatureManager manager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="manager"></param>
    public InboundReversalController(IMapper mapper, IFeatureManager manager)
    {
        this.mapper = mapper;
        this.manager = manager;
    }

    [HttpGet("isActive")]
    public async Task<bool> IsActive()
        => await manager.IsEnabledAsync(FeatureFlags.InboundReversal);

    /// <summary>
    /// Gets inbound reversals filtered by specified filters
    /// </summary>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpGet]
    [Produces("application/json")]
    public async Task<PaginatedList<InboundReversalDto>> GetInboundReversals([FromQuery] InboundReversalParameters parameters)
    {
        var gridFilter = mapper.Map<GetInboundReversalsDtoFilter>(parameters);
        return await Mediator.Send(new GetInboundReversalsQuery()
        {
            Pagination = parameters.Pagination,
            Filter = gridFilter,
            OrderByExpression = parameters.Sorting?.By,
            OrderByDescending = parameters.Sorting?.Descending ?? true
        });
    }

    /// <summary>
    /// Lists inbounds that can be reversed
    /// </summary>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpGet]
    [Route("getReversibleInbounds")]
    [Produces("application/json")]
    public async Task<PaginatedList<ReversibleInboundDto>> GetReversibleInbounds([FromQuery] GetReversibleInboundsDtoFilter filter)
    {
        return await Mediator.Send(new GetReversibleInboundsQuery()
        {
            Pagination = filter.Pagination,
            WhsCode = filter.WhsCode,
            InJobNo = filter.InJobNo,
            NewerThan = filter.NewerThan,
        });
    }

    /// <summary>
    /// Gets inbound reversal details based on specified job number
    /// </summary>
    /// <param name="jobNo">unique job number</param>
    /// <param name="userCode">user code - it is used by service filter</param>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpGet]
    [Route("details")]
    [Produces("application/json")]
    [ServiceFilter(typeof(CheckIfLockedFilter))]
    public async Task<InboundReversalDetailsDto?> GetInboundReversalDetails(string jobNo, string userCode)
    {
        return await Mediator.Send(new GetInboundReversalDetailsQuery()
        {
            JobNo = jobNo
        });
    }

    /// <summary>
    /// Creates the new inbound reversal object and returns it with default values filled
    /// </summary>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpPost]
    [Route("add")]
    [Produces("application/json")]
    public async Task<InboundReversal?> AddNewInboundReversal(string inJobNo, string userCode)
    {
        return await Mediator.Send(new AddNewInboundReversalCommand()
        {
            InJobNo = inJobNo,
            UserCode = userCode,
        });
    }

    /// <summary>
    /// Completes the inbound reversal
    /// </summary>
    /// <param name="jobNo">inbound reversal job number</param>
    /// <param name="userCode">user code - mandatory</param>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpPost]
    [Route("complete")]
    [Produces("application/json")]
    public async Task<InboundReversal> CompleteInboundReversal(string jobNo, string userCode)
    {
        return await Mediator.Send(new CompleteInboundReversalCommand()
        {
            JobNo = jobNo,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Sets the status of the inbound reversal to 'Cancelled'
    /// </summary>
    /// <param name="jobNo">inbound reversal job number</param>
    /// <param name="userCode">user code - mandatory</param>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpPost]
    [Route("cancel")]
    [Produces("application/json")]
    public async Task<InboundReversal> CancelInboundReversal(string jobNo, string userCode)
    {
        return await Mediator.Send(new CancelInboundReversalCommand()
        {
            JobNo = jobNo,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Changes reference no and remarks field of the inbound reversal object 
    /// </summary>
    /// <param name="jobNo">inbound reversal job number</param>
    /// <param name="refNo">reference number</param>
    /// <param name="reason">reason - optional</param>
    [FeatureGate(FeatureFlags.InboundReversal)]
    [HttpPost]
    [Route("update")]
    [Produces("application/json")]
    public async Task<InboundReversal> UpdateInboundReversal(string jobNo, string refNo, string? reason)
    {
        return await Mediator.Send(new UpdateInboundReversalCommand()
        {
            JobNo = jobNo,
            RefNo = refNo,
            Reason = reason
        });
    }
}



using Application.Common.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Presentation.Common;
using Presentation.Configuration;
using Application.UseCases.StockTransferReversals;
using Application.UseCases.StockTransferReversals.Queries.GetStockTransferReversals;
using Domain.Entities;
using Application.UseCases.StockTransferReversals.Commands.AddNewStockTransferReversalCommand;
using Application.UseCases.StockTransferReversals.Queries.GetReversibleStockTransfers;
using Application.UseCases.StockTransferReversals.Commands.CancelStockTransferReversalCommand;
using Presentation.Utilities;
using Application.UseCases.StockTransferReversals.Queries.GetStockTransferReversalDetails;
using Application.UseCases.StockTransferReversals.Commands.UpdateStockTransferReversalCommand;
using Application.UseCases.StockTransferReversals.Commands.CompleteStockTransferReversalCommand;

namespace Presentation.Controllers;

/// <summary>
/// StockTransferReversalController provides methods related to stock transfer reversals
/// </summary>
public partial class StockTransferReversalController : ApiControllerBase
{
    private readonly IMapper mapper;
    private readonly IFeatureManager manager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="manager"></param>
    public StockTransferReversalController(IMapper mapper, IFeatureManager manager)
    {
        this.mapper = mapper;
        this.manager = manager;
    }

    [HttpGet("isActive")]
    public async Task<bool> IsActive()
        => await manager.IsEnabledAsync(FeatureFlags.StockTransferReversal);

    /// <summary>
    /// Gets Stock Transfer reversals filtered by specified filters
    /// </summary>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpGet]
    public async Task<PaginatedList<StockTransferReversalDto>> GetStockTransferReversals([FromQuery] StockTransferParameters parameters)
    {
        var gridFilter = mapper.Map<GetStockTransferReversalsDtoFilter>(parameters);
        return await Mediator.Send(new GetStockTransferReversalsQuery()
        {
            Pagination = parameters.Pagination,
            Filter = gridFilter,
            OrderByExpression = parameters.Sorting?.By,
            OrderByDescending = parameters.Sorting?.Descending ?? true
        });
    }

    /// <summary>
    /// Lists stock transfers that can be reversed
    /// </summary>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpGet("getReversibleStockTransfers")]
    public async Task<PaginatedList<ReversibleStockTransferDto>> GetReversibleStockTransfers([FromQuery] GetReversibleStockTransfersDtoFilter filter)
    {
        return await Mediator.Send(new GetReversibleStockTransfersQuery()
        {
            Pagination = filter.Pagination,
            WhsCode = filter.WhsCode,
            StfJobNo = filter.StfJobNo,
            NewerThan = filter.NewerThan,
        });
    }

    /// <summary>
    /// Gets stock transfer reversal details based on specified job number
    /// </summary>
    /// <param name="jobNo">unique job number</param>
    /// <param name="userCode">user code - it is used by service filter</param>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpGet("details")]
    [ServiceFilter(typeof(CheckIfLockedFilter))]
    public async Task<StockTransferReversalDetailsDto> GetStockTransferReversalDetails(string jobNo, string userCode)
    {
        return await Mediator.Send(new GetStockTransferReversalDetailsQuery()
        {
            JobNo = jobNo
        });
    }

    /// <summary>
    /// Creates the new stock transfer reversal object and returns it with default values filled
    /// </summary>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpPost("add")]
    public async Task<StockTransferReversal> AddNewStockTransferReversal(string stfJobNo, string userCode)
    {
        return await Mediator.Send(new AddNewStockTransferReversalCommand()
        {
            StfJobNo = stfJobNo,
            UserCode = userCode,
        });
    }

    /// <summary>
    /// Completes the stock transfer reversal
    /// </summary>
    /// <param name="jobNo">stock transfer reversal job number</param>
    /// <param name="userCode">user code - mandatory</param>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpPost("complete")]
    public async Task<StockTransferReversal> CompleteStockTransferReversal(string jobNo, string userCode)
    {
        return await Mediator.Send(new CompleteStockTransferReversalCommand()
        {
            JobNo = jobNo,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Sets the status of the stock transfer reversal to 'Cancelled'
    /// </summary>
    /// <param name="jobNo">stock transfer reversal job number</param>
    /// <param name="userCode">user code - mandatory</param>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpPost("cancel")]
    public async Task<StockTransferReversal> CancelStockTransferReversal(string jobNo, string userCode)
    {
        return await Mediator.Send(new CancelStockTransferReversalCommand()
        {
            JobNo = jobNo,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Changes reference no and reason field of the stock transfer reversal object 
    /// </summary>
    /// <param name="jobNo">stock transfer reversal job number</param>
    /// <param name="refNo">reference number</param>
    /// <param name="reason">reason - optional</param>
    [FeatureGate(FeatureFlags.StockTransferReversal)]
    [HttpPost("update")]
    public async Task<StockTransferReversal> UpdateStockTransferReversal(string jobNo, string refNo, string? reason)
    {
        return await Mediator.Send(new UpdateStockTransferReversalCommand()
        {
            JobNo = jobNo,
            RefNo = refNo,
            Reason = reason
        });
    }
}



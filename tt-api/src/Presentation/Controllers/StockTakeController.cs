using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Presentation.Common;
using Presentation.Configuration;
using Application.UseCases.Common;
using Application.UseCases.Common.Queries.GetListQuery;
using Application.Common.Models;
using Application.UseCases.StockTake;
using Application.UseCases.StockTake.Queries.GetStockTakeList;
using Presentation.Utilities;
using Application.UseCases.StockTake.Commands.UpdateStockTake;
using Application.UseCases.StockTake.Queries.GetStockTakeAnotherLocPid;
using Application.UseCases.StockTake.Queries.GetStockTakeInvalidPid;
using Application.UseCases.StockTake.Queries.GetStockTakeMissingPid;
using Application.UseCases.StockTake.Queries.GetStockTakeUploadedList;
using Application.UseCases.StockTake.Commands.CompleteStockTake;
using Application.UseCases.StockTake.Commands.CancelStockTake;
using Application.UseCases.StockTake.Queries.GetStockTakeStandByLocations;
using Application.UseCases.StockTake.Commands.SendStandByNegative;

namespace Presentation.Controllers;
/// <summary>
/// StockTakeController provides method to get stock take data
/// </summary>
public partial class StockTakeController : ApiControllerBase
{
    private readonly IMapper mapper;
    private readonly IFeatureManager manager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="manager"></param>
    public StockTakeController(IMapper mapper, IFeatureManager manager)
    {
        this.mapper = mapper;
        this.manager = manager;
    }

    [HttpGet("isActive")]
    public async Task<bool> IsActive()
        => await manager.IsEnabledAsync(FeatureFlags.StockTake);

    /// <summary>
    /// Get stock take list
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpGet]
    public async Task<PaginatedList<StockTakeDto>> GetStockTakeList([FromQuery] GetStockTakeListParameters parameters)
    {
        var gridFilter = mapper.Map<GetStockTakeListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetStockTakeListDtoFilter, StockTakeDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.StockTake,
            Sorting = parameters.Sorting ?? new OrderBy { By = "jobno", Descending = true },
            Pagination = parameters.Pagination,
        });
        return result;
    }

    /// <summary>
    /// Gets stock  take
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpGet("getStockTake")]
    [ServiceFilter(typeof(CheckIfLockedFilter))]
#pragma warning disable IDE0060 // Remove unused parameter
    public async Task<StockTake> GetStockTake(string jobNo, string userCode)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return await Mediator.Send(new GetQuery<StockTake>()
        {
            Key = new string[] { jobNo },
            EntityType = EntityType.StockTake
        });
    }

    /// <summary>
    /// Updates stock take
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpPost("updateStockTake")]
    public async Task<StockTake> UpdateStockTake(UpdateStockTakeDto updated)
    {
        return await Mediator.Send(new UpdateStockTakeCommand()
        {
            Updated = updated
        });
    }

    /// <summary>
    /// Completes stock take
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpPost("completeStockTake")]
    public async Task<StockTake> CompleteStockTake(string jobNo, string userCode)
    {
        return await Mediator.Send(new CompleteStockTakeCommand()
        {
            JobNo = jobNo,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Cancels stock take
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpPost("cancelStockTake")]
    public async Task<StockTake> CancelStockTake(string jobNo, string userCode)
    {
        return await Mediator.Send(new CancelStockTakeCommand()
        {
            JobNo = jobNo,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Gets stock take uploaded data
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpGet("getAnotherLocPid")]
    public async Task<IEnumerable<StockTakeItemDto>> GetAnotherLocPid([FromQuery] StockTakeItemsParameters parameters)
    {
        var result = await Mediator.Send(new GetListQuery<GetStockTakeAnotherLocPidFilter, StockTakeItemDto>()
        {
            Filter = new GetStockTakeAnotherLocPidFilter { JobNo = parameters.JobNo },
            EntityType = EntityType.StockTakeItem,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Gets stock take invalid pid data
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpGet("getInvalidPid")]
    public async Task<IEnumerable<StockTakeItemDto>> GetInvalidPid([FromQuery] StockTakeItemsParameters parameters)
    {
        var result = await Mediator.Send(new GetListQuery<GetStockTakeInvalidPidFilter, StockTakeItemDto>()
        {
            Filter = new GetStockTakeInvalidPidFilter { JobNo = parameters.JobNo },
            EntityType = EntityType.StockTakeItem,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Gets stock take missing pid data
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpGet("getMissingPid")]
    public async Task<IEnumerable<StockTakeItemDto>> GetMissingPid([FromQuery] StockTakeItemsParameters parameters)
    {
        var result = await Mediator.Send(new GetListQuery<GetStockTakeMissingPidFilter, StockTakeItemDto>()
        {
            Filter = new GetStockTakeMissingPidFilter { JobNo = parameters.JobNo },
            EntityType = EntityType.StockTakeItem,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Gets stock take uploaded data
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpGet("getUploadedList")]
    public async Task<IEnumerable<StockTakeItemDto>> GetUploadedList([FromQuery] StockTakeItemsParameters parameters)
    {
        var result = await Mediator.Send(new GetListQuery<GetStockTakeUploadedListFilter, StockTakeItemDto>()
        {
            Filter = new GetStockTakeUploadedListFilter { JobNo = parameters.JobNo },
            EntityType = EntityType.StockTakeItem,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Gets stock take location list for stand-by
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpGet("getStandByLocations")]
    public async Task<IEnumerable<StockTakeLocationDto>> GetStandByLocations(string jobNo)
    {
        var result = await Mediator.Send(new GetListQuery<GetStockTakeStandByLocationsFilter, StockTakeLocationDto>()
        {
            Filter = new GetStockTakeStandByLocationsFilter { JobNo = jobNo },
            EntityType = EntityType.Location,
            Sorting = new OrderBy { By = "Code", Descending = false },
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Send stand-by negative
    /// </summary>
    [FeatureGate(FeatureFlags.StockTake)]
    [HttpPost("sendStandByNegative")]
    public async Task<ActionResult> SendStandByNegative(string jobNo, string locationCode, string whsCode, string userCode)
    {
        await Mediator.Send(new SendStandByNegativeCommand()
        {
            JobNo = jobNo,
            LocationCode = locationCode,
            WhsCode = whsCode,
            UserCode = userCode
        });
        return Ok();
    }


}



using Application.Common.Models;
using Application.UseCases.Adjustments.Commands.AddNewAdjustmentCommand;
using Application.UseCases.Adjustments.Commands.CancelAdjustmentCommand;
using Application.UseCases.Adjustments.Commands.CompleteAdjustmentCommand;
using Application.UseCases.Adjustments.Commands.UpdateAdjustmentCommand;
using Application.UseCases.Adjustments.Queries.GetAdjustmentPalletsInILog;
using Application.UseCases.Adjustments.Queries.GetAdjustmentsQuery;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Presentation.Utilities;

namespace Presentation.Controllers;

/// <summary>
/// AdjustmentController provides methods to get the adjustment list and adjustment detail methods (show details, add new, modify, delete)
/// </summary>
public partial class AdjustmentController : ApiControllerBase
{
    private readonly IMapper mapper;
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public AdjustmentController( IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets inventory adjustments paged list per warehouse code; results can be also filtered by specific field values
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public async Task<PaginatedList<Adjustment>> GetAdjustments([FromQuery] AdjustmentParameters filter, string whsCode)
    {
        var gridFilter = mapper.Map<AdjustmentFilter>(filter);
        gridFilter.WhsCode = whsCode;
        return await Mediator.Send(new GetAdjustmentsQuery()
        {
            Filter = gridFilter,
            Pagination = filter.Pagination,
            OrderByExpression = filter.Sorting?.By,
            OrderByDescending = filter.Sorting?.Descending ?? false,

        }); 
    }

    /// <summary>
    /// Gets the adjustment detail object based on specified job number
    /// </summary>
    /// <param name="jobNo">unique job number</param>
    /// <param name="userCode">user code - it is used by service filter</param>
    [HttpGet]
    [Route("details")]
    [Produces("application/json")]
    [ServiceFilter(typeof(CheckIfLockedFilter))]
    public async Task<Adjustment?> GetAdjustmentDetails(string jobNo, string userCode)
    {
        return await Mediator.Send(new GetAdjustmentDetailsQuery()
        {
            JobNo = jobNo
        });
    }

    /// <summary>
    /// Creates the new adjustment object and returns it with default values filled
    /// </summary>
    /// <param name="whsCode">warehouse code - mandatory</param>
    /// <param name="customerCode">customer code - mandatory</param>
    /// <param name="userCode">user code - mandatory</param>
    /// <param name="isUndoZeroOut"> this flag determines Job Type for the new adjustment; if it is set to false then JobType is set to normal otherwise to UndoZeroOut  </param>
    [HttpGet]
    [Route("add")]
    [Produces("application/json")]
    public async Task<Adjustment?> AddNewAdjustment(string whsCode, string customerCode, string userCode, bool isUndoZeroOut = false)
    {
        var jobNo = await Mediator.Send(new AddNewAdjustmentCommand()
        {
           WhsCode = whsCode,
           CustomerCode = customerCode,
           UserCode = userCode,
           IsUndoZeroOut = isUndoZeroOut
        });
        return await Mediator.Send(new GetAdjustmentDetailsQuery()
        { JobNo = jobNo });
    }

    /// <summary>
    /// Changes the status of the adjustment to Completed
    /// </summary>
    /// <param name="jobNo">unique job number</param>
    /// <param name="userCode">user code - mandatory</param>
    [HttpPost]
    [Route("complete")]
    [Produces("application/json")]
    public async Task<Adjustment> CompleteAdjustment(string jobNo, string userCode)
    {
        return await Mediator.Send(new CompleteAdjustmentCommand()
        {
            JobNo = jobNo,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Changes reference no and remarks fields of the adjustment 
    /// </summary>
    /// <param name="adjustment">represents updated object</param>
    [HttpPost]
    [Route("update")]
    [Produces("application/json")]
    public async Task<Adjustment> UpdateAdjustment(UpdatedAdjustmentVM adjustment)
    {
        return await Mediator.Send(new UpdateAdjustmentCommand()
        {
            Adjustment = adjustment
        });
    }

    /// <summary>
    /// Sets the status of the adjustment to 'Cancelled'
    /// </summary>
    /// <param name="jobNo">unique job number</param>
    /// <param name="userCode">user code - mandatory</param>
    [HttpPost]
    [Route("cancel")]
    [Produces("application/json")]
    public async Task<Adjustment> CancelAdjustment(string jobNo, string userCode)
    {
        return await Mediator.Send(new CancelAdjustmentCommand()
        {
            JobNo = jobNo,
            UserCode=userCode
        });
    }

    /// <summary>
    /// Gets pallets on inventory adjustment which are on iLog Storage locations
    /// </summary>
    /// <param name="jobNo">Adjustment job number</param>
    /// <returns></returns>
    [HttpGet("getPalletsInILog")]
    public async Task<object> GetPalletsInILog(string jobNo)
    {
        return await Mediator.Send(new GetAdjustmentPalletsInILogQuery() { JobNo = jobNo });
    }
}


 
using Application.Common.Models;
using Application.UseCases.AdjustmentItems;
using Application.UseCases.AdjustmentItems.Commands.DeleteAdjustmentItemCommand;
using Application.UseCases.AdjustmentItems.Commands.PrepareNewAdjustmentItemCommand;
using Application.UseCases.AdjustmentItems.Commands.UpdateAdjustmentItemCommand;
using Application.UseCases.AdjustmentItems.Queries.GetAdjustmentItemQuery;
using Application.UseCases.AdjustmentItems.Queries.GetAdjustmentItemsQuery;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;

namespace Presentation.Controllers;

/// <summary>
/// AdjustmentItemsController provides methods to get the adjustment item list and adjustment item detail methods (show details, add new, modify, delete)
/// </summary>
public partial class AdjustmentItemsController : ApiControllerBase
{
    private readonly IMapper mapper;
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public AdjustmentItemsController( IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets list of adjustment items per specified job number
    /// </summary>
    /// <param name="jobNo">adjustment job number - mandatory</param>
    /// <param name="sorting">specifies how data should be sorted in the result set</param>
    [HttpGet]
    [Produces("application/json")]
    public async Task<List<AdjustmentItem>> Get(string jobNo, [FromQuery] OrderBy? sorting)
    {
        return await Mediator.Send(new GetAdjustmentItemsQuery()
        {
            JobNo = jobNo,
            OrderByExpression = sorting?.By,
            OrderByDescending = sorting?.Descending ?? false
        }); 
    }

    /// <summary>
    /// Gets adjustment item details
    /// </summary>
    /// <param name="jobNo">adjustment job number - mandatory</param>
    /// <param name="lineItem">adjustment line number - mandatory</param>
    [HttpGet]
    [Route("details")]
    [Produces("application/json")]
    public async Task<AdjustmentItemWithPalletDto> Get(string jobNo, int lineItem)
    {
        return await Mediator.Send(new GetAdjustmentItemQuery()
        {
            JobNo = jobNo,
            LineItem = lineItem
        });
    }

    /// <summary>
    /// Pre-creates new adjustment item details based on PID number
    /// </summary>
    /// <param name="PID">pallet identifier - mandatory</param>
    /// <param name="jobNo">adjustment job number - mandatory</param>
    [HttpGet]
    [Route("prepareNew")]
    [Produces("application/json")]
    public async Task<AdjustmentItemWithPalletDto> PrepareNew(string PID, string jobNo)
    {
        return await Mediator.Send(new PrepareNewAdjustmentItemCommand()
        {
            JobNo = jobNo,
            PID = PID
        });
    }

    /// <summary>
    /// Updates adjustment item - can be used to update existing adjustment item object or 
    /// saves the new one (in this case LineItem should stay 0). 
    /// </summary>
    /// <param name="adjustmentItem">adjustment item</param>
    /// <param name="userCode">user code - mandatory</param>
    [HttpPost]
    public async Task<IActionResult> Update(AdjustmentItemDto adjustmentItem, string userCode)
    {
        await Mediator.Send(new UpdateAdjustmentItemCommand()
        {
            AdjustmentItem = adjustmentItem,
            UserCode = userCode
        });
        return Ok();
    }


    /// <summary>
    /// Deletes adjustment item
    /// </summary>
    /// <param name="jobNo">deleted adjustment job number - mandatory</param>
    /// <param name="lineItem">deleted adjustment line number - mandatory</param>
    [HttpDelete]
    public async Task<IActionResult> Delete(string jobNo, int lineItem)
    {
        await Mediator.Send(new DeleteAdjustmentItemCommand()
        {
            JobNo = jobNo,
            LineItem = lineItem
        });
        return Ok();
    }
}


 
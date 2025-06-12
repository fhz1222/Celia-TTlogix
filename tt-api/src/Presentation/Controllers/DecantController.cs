using Application.Common.Models;
using Application.UseCases.Decants;
using Application.UseCases.Decants.Commands.AddNewDecantCommand;
using Application.UseCases.Decants.Commands.AddNewDecantItemCommand;
using Application.UseCases.Decants.Commands.CancelDecantCommand;
using Application.UseCases.Decants.Commands.CompleteDecantCommand;
using Application.UseCases.Decants.Commands.DeleteDecantItemCommand;
using Application.UseCases.Decants.Commands.UpdateDecantCommand;
using Application.UseCases.Decants.Queries.GetDecantDetail;
using Application.UseCases.Decants.Queries.GetDecants;
using Application.UseCases.Decants.Queries.GetPalletForDecant;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Presentation.Utilities;

namespace Presentation.Controllers;

/// <summary>
/// DecantController provides methods to get the list of decants and decant detail methods (show details, add new, modify, delete)
/// </summary>
public partial class DecantController : ApiControllerBase
{
    private readonly IMapper mapper;
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public DecantController( IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets a paged list of decants per warehouse code; results can be also filtered by specific field values
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public async Task<PaginatedList<DecantDto>> GetDecants([FromQuery] DecantParameters filter, string whsCode)
    {
        var gridFilter = mapper.Map<DecantDtoFilter>(filter);
        gridFilter.WhsCode = whsCode;
        return await Mediator.Send(new GetDecantsQuery()
        {
            Filter = gridFilter,
            Pagination = filter.Pagination,
            OrderByExpression = filter.Sorting?.By,
            OrderByDescending = filter.Sorting?.Descending ?? false,

        }); 
    }

    /// <summary>
    /// Gets decant details based on specified job number
    /// </summary>
    /// <param name="jobNo">unique job number</param>
    /// <param name="userCode">user code - it is used by service filter</param>
    [HttpGet]
    [Route("details")]
    [Produces("application/json")]
    [ServiceFilter(typeof(CheckIfLockedFilter))]
    public async Task<Decant?> GetDecantDetails(string jobNo, string userCode)
    {
        return await Mediator.Send(new GetDecantDetailsQuery()
        {
            JobNo = jobNo
        });
    }
    
    /// <summary>
    /// Creates the new decant object and returns it with default values filled
    /// </summary>
    /// <param name="whsCode">warehouse code - mandatory</param>
    /// <param name="customerCode">customer code - mandatory</param>
    /// <param name="userCode">user code - mandatory</param>
    [HttpGet]
    [Route("add")]
    [Produces("application/json")]
    public async Task<Decant?> AddNewDecant(string whsCode, string customerCode, string userCode)
    {
        return await Mediator.Send(new AddNewDecantCommand()
        {
           WhsCode = whsCode,
           CustomerCode = customerCode,
           UserCode = userCode
        });
    }

    /// <summary>
    /// Changes reference no and remarks field of the decant object 
    /// </summary>
    /// <param name="jobNo">decant job number</param>
    /// <param name="referenceNo">reference number - optional</param>
    /// <param name="remark">remark - optional</param>
    [HttpPost]
    [Route("update")]
    [Produces("application/json")]
    public async Task<Decant> UpdateDecant(string jobNo, string? referenceNo, string? remark)
    {
        return await Mediator.Send(new UpdateDecantCommand()
        {
            JobNo = jobNo,
            ReferenceNo = referenceNo,
            Remark = remark
        });
    }

    
    /// <summary>
    /// Sets the status of the decant to 'Cancelled'
    /// </summary>
    /// <param name="jobNo">decant job number</param>
    /// <param name="userCode">user code - mandatory</param>
    [HttpPost]
    [Route("cancel")]
    [Produces("application/json")]
    public async Task<Decant> CancelDecant(string jobNo, string userCode)
    {
        return await Mediator.Send(new CancelDecantCommand()
        {
            JobNo = jobNo,
            UserCode=userCode
        });
    }
    
    /// <summary>
    /// Executes decant action and sets decant status to Completed
    /// </summary>
    /// <param name="jobNo">decant job number</param>
    /// <param name="userCode">user code - mandatory</param>
    [HttpPost]
    [Route("complete")]
    [Produces("application/json")]
    public async Task<Decant> CompleteDecant(string jobNo, string userCode)
    {
        await Mediator.Send(new CompleteDecantCommand()
        {
            JobNo = jobNo,
            UserCode = userCode
        });
        return await Mediator.Send(new GetDecantDetailsQuery()
        {
            JobNo = jobNo
        });
    }

    /// <summary>
    /// Creates the new decant item object; returns updated decant object
    /// </summary>
    /// <param name="newDecantItem">decant item object -specifies about decant job number, pallet identifier and new quantities</param>
    /// <param name="userCode">user code - mandatory</param>
    [HttpPost]
    [Route("addItem")]
    [Produces("application/json")]
    public async Task<Decant> AddNewDecantItem(DecantItemDto newDecantItem, string userCode)
    {
        await Mediator.Send(new AddNewDecantItemCommand()
        {
            JobNo = newDecantItem.JobNo,
            PalletId = newDecantItem.PID,
            UserCode = userCode,
            NewQuantities = newDecantItem.NewQuantities
        });
        return await Mediator.Send(new GetDecantDetailsQuery
        { JobNo = newDecantItem.JobNo });
    }
    /// <summary>
    /// Deletes decant item object; returns updated decant object
    /// </summary>
    /// <param name="jobNo">decant job number</param>
    /// <param name="pid">pallet identifier which should be removed from the decant object</param>
    [HttpDelete]
    [Route("deleteItem")]
    [Produces("application/json")]
    public async Task<Decant> DeleteDecantItem(string jobNo, string pid)
    {
        await Mediator.Send(new DeleteDecantItemCommand()
        {
            JobNo = jobNo,
            PalletId = pid,
        });
        return await Mediator.Send(new GetDecantDetailsQuery
        { JobNo = jobNo });
    }

    /// <summary>
    /// Loads pallet for decant
    /// </summary>
    /// <param name="jobNo">unique job number</param>
    /// <param name="pid">pallet identifier</param>
    [HttpGet]
    [Route("pallet")]
    [Produces("application/json")]
    public async Task<Pallet> GetPallet(string jobNo, string pid)
    {
        return await Mediator.Send(new GetPalletForDecantQuery()
        {
            JobNo = jobNo,
            PID = pid
        });
    }
}



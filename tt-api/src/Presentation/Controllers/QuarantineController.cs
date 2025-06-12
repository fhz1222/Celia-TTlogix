using Application.Common.Models;
using Application.UseCases.Quarantine;
using Application.UseCases.Quarantine.Commands.UpdateQuarantineReason;
using Application.UseCases.Quarantine.Queries.GetQuarantineItems;
using Application.UseCases.Quarantine.Queries.GetQuarantinePalletsInILog;
using Application.UseCases.Quarantine.Queries.GetQuarantineReason;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;

namespace Presentation.Controllers;
/// <summary>
/// QuarantineControler provides quantine data
/// </summary>
public partial class QuarantineController : ApiControllerBase
{
    private readonly IMapper mapper;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public QuarantineController( IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets quarantine data filtered by specified filters
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public async Task<PaginatedList<QuarantineItemDto>> GetQuarantineItems([FromQuery] QuarantineParameters filter, string whsCode)
    {
        var gridFilter = mapper.Map<QuarantineItemDtoFilter>(filter);
        gridFilter.WhsCode = whsCode;
        return await Mediator.Send(new GetQuarantineItemsQuery()
        {
            Pagination = filter.Pagination,
            Filter = gridFilter,
            OrderByExpression = filter.Sorting?.By,
            OrderByDescending = filter.Sorting?.Descending ?? false,
            
        });
    }

    /// <summary>
    /// Gets quarantine reason for given pallet indentifiers; if the reason text is not the same for all PIDs the method throws exception
    /// </summary>
    /// <param name="pids">table of pallet identifiers</param>
    /// <returns>quarantine reason text or null if quarantine reason does not exist </returns>
    [HttpGet]
    [Route("reason")]
    [Produces("application/json")]
    public async Task<string?> GetQuarantineReason(string PID)
    {
        return await Mediator.Send(new GetQuarantineReasonQuery()
        {
            PID = PID
        });
    }

    /// <summary>
    /// Sets quarantine reason for given pallet indentifiers
    /// </summary>
    /// <param name="pids">table of pallet identifiers</param>
    /// <param name="reason">quarantine reason text</param>
    [HttpPost]
    [Route("updateReason")]
    [Produces("application/json")]
    public async Task<IActionResult> UpdateQuarantineReason(QuarantineReasonUpdateDto dto)
    {
        await Mediator.Send(new UpdateQuarantineReasonCommand()
        {
            PIDS = dto.PIDS,
            Reason = dto.Reason
        });
        return Ok();
    }

    /// <summary>
    /// Gets pallets on quarantine job which are on iLog Storage locations
    /// </summary>
    /// <param name="jobNo">Quarantine job number</param>
    /// <returns>List of quarantined PIDs</returns>
    [HttpGet("getPalletsInILog")]
    public async Task<IEnumerable<QuarantinePalletDto>> GetPalletsInILog(string jobNo)
    {
        return await Mediator.Send(new GetQuerantinePalletsInILogQuery()
        {
            JobNo = jobNo
        });
    }
}



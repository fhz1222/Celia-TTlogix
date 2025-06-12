using Application.Common.Models;
using Application.UseCases.RelocationLogs;
using Application.UseCases.RelocationLogs.Queries.GetRelocationLogs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;

namespace Presentation.Controllers;

/// <summary>
/// RelocationLogControler provides relocation log data
/// </summary>
public partial class RelocationLogController : ApiControllerBase
{
    private readonly IMapper mapper;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public RelocationLogController( IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets relocation log data filtered by specified filters
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public async Task<PaginatedList<RelocationLogDto>> GetRelocationLogs([FromQuery] RelocationLogParameters filter)
    {
        return await Mediator.Send(new GetRelocationLogsQuery()
        {
            Pagination = filter.Pagination,
            Filter = mapper.Map<RelocationLogDtoFilter>(filter),
            OrderByExpression = filter.Sorting?.By,
            OrderByDescending = filter.Sorting?.Descending ?? false,
            
        });
    }
}



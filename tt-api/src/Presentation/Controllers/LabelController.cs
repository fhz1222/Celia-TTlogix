using Application.UseCases.Labels;
using Application.UseCases.Labels.Queries.GetVmiLabel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
using System.Text.Json;

namespace Presentation.Controllers;

/// <summary>
/// LabelControler provides relocation log data
/// </summary>
public partial class LabelController : ApiControllerBase
{
    private readonly IMapper mapper;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public LabelController( IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets relocation log data filtered by specified filters
    /// </summary>
    [HttpGet]
    [Route("vmiLabel")]
    [Produces("application/json")]
    public async Task<VmiLabelDto> GetVmiLabelData([FromQuery] string pid)
    {
        return await Mediator.Send(new GetVmiLabelQuery()
        {
            Pid = pid
        });
    }
}



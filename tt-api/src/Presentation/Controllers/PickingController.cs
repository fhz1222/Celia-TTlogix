using Application.Exceptions;
using Application.UseCases.Picking.PickPallet;
using AutoMapper;
using Presentation.Common;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// PickingController provides methods related to pallet picking
/// </summary>
public partial class PickingController : ApiControllerBase
{
    private readonly IMapper mapper;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public PickingController(IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Registers pallet pick
    /// </summary>
    [HttpPost]
    [Route("PickPallet")]
    public async Task PickPallet([FromBody] PickPalletCommand command)
    {
        await Mediator.Send(command);
    }
}



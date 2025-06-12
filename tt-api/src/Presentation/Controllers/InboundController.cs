using Application.UseCases.Inbound.Commands.UndoPutaway;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// InboundController provides methods to manage inbounds
/// </summary>
public partial class InboundController : ApiControllerBase
{
    /// <summary>
    /// Reverts putaway of selected pallets on inbound
    /// </summary>
    /// <param name="pids">List of PIDs from the same inbound</param>
    /// <returns></returns>
    [HttpPost("undoPutaway")]
    public async Task UndoPutaway([FromBody] string[] pids)
        => await Mediator.Send(new UndoPutawayCommand() { Pids = pids });
}

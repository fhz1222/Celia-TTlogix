using Application.UseCases.ILogLocationIntegration;
using Application.UseCases.ILogLocationIntegration.Commands.ActivateLocationsCommand;
using Application.UseCases.ILogLocationIntegration.Commands.AddLocationsCommand;
using Application.UseCases.ILogLocationIntegration.Commands.DeactivateLocationsCommand;
using Application.UseCases.ILogLocationIntegration.Commands.UpdateLocationsCommand;
using Application.UseCases.ILogLocationIntegration.Queries.GetLocationsForWHS;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// ILogLocationIntegration functionality regarding location integration with ilog
/// </summary>
public partial class ILogLocationIntegrationController : ApiControllerBase
{
    /// <summary>
    /// Returns list of locations in warehouses
    /// </summary>
    [HttpGet("locationsforwhs")]
    [Produces("application/json")]
    public async Task<IEnumerable<ILogIntegrationLocationDto>> GetLocationsForWHS([FromQuery] string[] WHSCodes)
    {
        return await Mediator.Send(new GetLocationsForWHSQuery()
        {
            WHSCodes = WHSCodes,
        });
    }

    /// <summary>
    /// Adds a list of locations
    /// </summary>
    [HttpPost("add")]
    [Produces("application/json")]
    public async Task Add(IEnumerable<ILogIntegrationLocationDto> items)
    {
        await Mediator.Send(new AddLocationsCommand { Items = items });
    }

    /// <summary>
    /// Deactivates a list of locations
    /// </summary>
    [HttpPost("activate")]
    [Produces("application/json")]
    public async Task Activate(IEnumerable<ILogIntegrationLocationIdDto> items)
    {
        await Mediator.Send(new ActivateLocationsCommand { Items = items });
    }

    /// <summary>
    /// Deactivates a list of locations
    /// </summary>
    [HttpPost("deactivate")]
    [Produces("application/json")]
    public async Task Deactivate(IEnumerable<ILogIntegrationLocationIdDto> items)
    {
        await Mediator.Send(new DeactivateLocationsCommand { Items = items });
    }

    /// <summary>
    /// Updates a list of locations
    /// </summary>
    [HttpPost("update")]
    [Produces("application/json")]
    public async Task Update(IEnumerable<ILogIntegrationLocationDto> items)
    {
        await Mediator.Send(new UpdateLocationsCommand { Items = items });
    }
}



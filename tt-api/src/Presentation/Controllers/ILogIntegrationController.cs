using Application.UseCases.ILogIntegration;
using Application.UseCases.ILogIntegration.Commands.CreatePalletFromBox;
using Application.UseCases.ILogIntegration.Commands.DeleteBoxes;
using Application.UseCases.ILogIntegration.Commands.Disable;
using Application.UseCases.ILogIntegration.Commands.Enable;
using Application.UseCases.ILogIntegration.Commands.GenerateBoxes;
using Application.UseCases.ILogIntegration.Commands.UpdateBoxes;
using Application.UseCases.ILogIntegration.Queries.GetBoxes;
using Application.UseCases.ILogIntegration.Queries.GetConfig;
using Application.UseCases.ILogIntegration.Queries.IsActiveForWarehouse;
using Application.UseCases.ILogIntegration.Queries.TryGetBoxes;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// ILogIntegrationController provides methods to manage integration with iLog WMS
/// </summary>
public class ILogIntegrationController : ApiControllerBase
{
    /// <summary>
    /// Returns current iLog integration configuration values - status and warehouse codes
    /// </summary>
    /// <returns>Current configuration</returns>
    [HttpGet("config")]
    [Produces("application/json")]
    public async Task<IntegrationConfigDto> GetConfig()
    {
        return await Mediator.Send(new GetConfigQuery());
    }

    /// <summary>
    /// Returns whether integration with iLog is currently active in the given warehouse
    /// </summary>
    /// <param name="whsCode">Warehouse code</param>
    /// <returns>Is iLog integration active in warehouse</returns>
    [HttpGet("isActiveForWarehouse")]
    public async Task<bool> GetIfActiveForWarehouse(string whsCode)
    {
        return await Mediator.Send(new IsActiveForWarehouseQuery() { WHSCode = whsCode });
    }

    /// <summary>
    /// Disables integration with iLog
    /// </summary>
    /// <returns></returns>
    [HttpPost("disable")]
    public async Task Disable()
    {
        await Mediator.Send(new DisableCommand());
    }

    /// <summary>
    /// Enables integration with iLog
    /// </summary>
    /// <param name="whsCode"></param>
    /// <returns></returns>
    [HttpPost("enable")]
    public async Task Enable(string whsCode)
    {
        await Mediator.Send(new EnableCommand() { WHSCode = whsCode });
    }

    /// <summary>
    /// Generates boxes for iLog integration
    /// </summary>
    /// <param name="palletIds">List of PIDs for which boxes should be generated</param>
    /// <returns>List of created boxes</returns>
    [HttpPost("generateBoxes")]
    public async Task<List<BoxDto>> GenerateBoxes([FromQuery] string[] palletIds)
    {
        var result = new List<BoxDto>();
        foreach (var palletId in palletIds)
        {
            var boxes = await Mediator.Send(new GenerateBoxesCommand() { Pid = palletId });
            result.AddRange(boxes);
        }
        return result;
    }

    /// <summary>
    /// Returns boxes which are integrated for CPart pallet
    /// </summary>
    /// <param name="palletId">PID</param>
    /// <returns>List of boxes for PID</returns>
    [HttpGet("getBoxes")]
    public async Task<List<BoxDto>> GetBoxes(string palletId)
    {
        return await Mediator.Send(new GetBoxesQuery() { Pid = palletId });
    }

    /// <summary>
    /// Returns boxes integrated for pallets (where applicable)
    /// </summary>
    /// <param name="pids">PIDs</param>
    /// <returns>List of boxes for PIDs</returns>
    [HttpGet("tryGetBoxes")]
    public async Task<List<BoxDto>> TryGetBoxes([FromQuery] string[] pids)
    {
        return await Mediator.Send(new TryGetBoxesQuery() { Pids = pids });
    }

    /// <summary>
    /// Updates quantities on boxes
    /// </summary>
    /// <param name="boxes">List of box DTOs</param>
    /// <returns></returns>
    [HttpPost("updateBoxes")]
    public async Task UpdateBoxes([FromBody] BoxDto[] boxes)
    {
        await Mediator.Send(new UpdateBoxesCommand() { Boxes = boxes });
    }

    /// <summary>
    /// Deletes boxes
    /// </summary>
    /// <param name="boxIds">Identifiers of boxes to delete</param>
    /// <returns></returns>
    [HttpPost("deleteBoxes")]
    public async Task DeleteBoxes([FromBody] string[] boxIds)
    {
        await Mediator.Send(new DeleteBoxesCommand() { Boxes = boxIds });
    }

    /// <summary>
    /// Repacking box into a new pallet
    /// </summary>
    /// <param name="boxId">Box identifier which needs separating out</param>
    /// <returns>PID of created pallet</returns>
    [HttpPost("createPalletFromBox")]
    public async Task<string> CreatePalletFromBox([FromBody] string boxId)
    {
        return await Mediator.Send(new CreatePalletFromBoxCommand() { BoxId = boxId });
    }
}
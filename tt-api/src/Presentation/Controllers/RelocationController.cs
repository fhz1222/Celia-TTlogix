using Application.UseCases.Relocation.Commands;
using Application.UseCases.RelocationLogs.Queries.GetRelocationLogs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;

namespace Presentation.Controllers;

/// <summary>
/// RelocationControler relocates pallets
/// </summary>
public partial class RelocationController : ApiControllerBase
{

    /// <summary>
    /// Relocates pallet and returns relocated pallet
    /// </summary>
    [HttpPost("forcerelocate")]
    [Produces("application/json")]
    public async Task<Pallet> ForceRelocatePallet(RelocatePalletDto relocatePallet, string userCode)
    {
        return await Mediator.Send(new ForceRelocatePalletFromiLogCommand()
        {
            PID = relocatePallet.PID,
            NewLocation = relocatePallet.NewLocation,
            RelocatedBy = userCode,
            RelocatedOn = relocatePallet.RelocatedOn,
            AllowedTrgLocationCategory = relocatePallet.AllowedTrgLocationCategory
        });
    }
}



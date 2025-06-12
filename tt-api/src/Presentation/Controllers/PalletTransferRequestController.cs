using Application.UseCases.PalletTransferRequests;
using Application.UseCases.PalletTransferRequests.Commands.AddNewPalletTransferRequestCommand;
using Application.UseCases.PalletTransferRequests.Queries.GetOngoingPalletTransferRequests;
using Application.UseCases.PalletTransferRequests.Queries.GetPalletTransferRequest;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;

namespace Presentation.Controllers;

/// <summary>
/// PalletTransferRequestController provides methods to manage Pallet Transfer Requests
/// </summary>
public class PalletTransferRequestController : ApiControllerBase
{
    /// <summary>
    /// Gets pallet transfer request by identifier
    /// </summary>
    /// <param name="jobNo">Pallet Transfer Request identifier</param>
    /// <returns>Pallet Transfer Request details</returns>
    [HttpGet]
    [Produces("application/json")]
    public async Task<PalletTransferRequestDto> GetPalletTransferRequest([FromQuery] string jobNo)
    {
        return await Mediator.Send(new GetPalletTransferRequestQuery() { JobNo = jobNo });
    }

    /// <summary>
    /// Gets not closed Pallet Transfer Requests
    /// </summary>
    /// <returns>List of Pallet Transfer Requests</returns>
    [HttpGet]
    [Route("ongoing")]
    [Produces("application/json")]
    public async Task<IEnumerable<PalletTransferRequest>> GetOngoingPalletTransferRequest()
    {
        return await Mediator.Send(new GetOngoingPalletTransferRequestsQuery ());
    }

    /// <summary>
    /// Creates new Pallet Transfer Requests and integrates them with iLog
    /// </summary>
    /// <param name="palletTransfersRequest">Object with PIDs to request</param>
    /// <param name="userCode">User code requesting the PTRs</param>
    /// <returns></returns>
    [HttpPost]
    [Route("add")]
    [Produces("application/json")]
    public async Task AddNewPalletTransferRequest(NewPalletTransfersRequestDto palletTransfersRequest, string userCode)
    {
        foreach (var PID in palletTransfersRequest.PIDs)
        {
            await Mediator.Send(new AddNewPalletTransferRequestCommand()
            {
                Pid = PID, 
                UserCode = userCode
            });
        }
    }

    /// <summary>
    /// Closes Pallet Transfer Request
    /// </summary>
    /// <param name="request">Object with PTR completion details</param>
    /// <returns></returns>
    [HttpPost]
    [Route("complete")]
    [Produces("application/json")]
    public async Task CompletePalletTransferRequest(CompletePalletTransferRequestCommand request)
    {
        await Mediator.Send(request);
    }


}



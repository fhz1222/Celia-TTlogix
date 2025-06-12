using Application.UseCases.ILogPickingRequests;
using Application.UseCases.ILogPickingRequests.Commands.ClosePickingRequest;
using Application.UseCases.ILogPickingRequests.Commands.NewOutboundPickingRequest;
using Application.UseCases.ILogPickingRequests.Queries.GetOpenPickingRequest;
using Application.UseCases.ILogPickingRequests.Queries.GetPickingRequest;
using Application.UseCases.Loadings.Queries.GetIncompleteILogOutbounds;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;

namespace Presentation.Controllers;

/// <summary>
/// ILogPickingRequestController provides methods to manage picking requests for iLog
/// </summary>
public partial class ILogPickingRequestController : ApiControllerBase
{
    /// <summary>
    /// Raises a new picking request for outbound and integrates it with iLog
    /// </summary>
    /// <param name="dto">Object containing outbound job number</param>
    /// <param name="userCode">User raising the request</param>
    /// <returns>Created picking request</returns>
    [HttpPost("requestOutboundPicking")]
    public async Task<CreatedOutboundPickingRequestDto> RequestOutboundPicking(RequestOutboundPickingDto dto, string userCode)
    {
        return await Mediator.Send(new NewOutboundPickingRequestCommand
        {
            OutboundJobNo = dto.JobNo,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Raises new picking requests for outbounds on loading and integrates them with iLog
    /// </summary>
    /// <param name="dto">Object containing loading job number</param>
    /// <param name="userCode">User raising the request</param>
    /// <returns>List of created picking requests</returns>
    [HttpPost("requestLoadingPicking")]
    public async Task<List<CreatedOutboundPickingRequestDto>> RequestLoadingPicking(RequestLoadingPickingDto dto, string userCode)
    {
        return await Mediator.Send(new NewLoadingPickingRequestCommand
        {
            LoadingJobNo = dto.JobNo,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Returns open picking request identifier for outbound
    /// </summary>
    /// <param name="outboundJobNo">Outbound job number</param>
    /// <returns>Picking request identifier or NULL if no open picking request exist</returns>
    [HttpGet("openPickingRequest")]
    public async Task<string?> GetOpenPickingRequest(string outboundJobNo)
    {
        return await Mediator.Send(new GetOpenPickingRequestQuery
        {
            OutboundJobNo = outboundJobNo
        });
    }

    /// <summary>
    /// Gets details of picking request revision
    /// </summary>
    /// <param name="pickingRequestId">Picking request identifier</param>
    /// <param name="revision">Picking request revision</param>
    /// <returns>Picking request details</returns>
    [HttpGet]
    public async Task<PickingRequestDto> GetPickingRequest(string pickingRequestId, int revision)
    {
        return await Mediator.Send(new GetPickingRequestQuery() { PickingRequestId = pickingRequestId, Revision = revision });
    }

    /// <summary>
    /// Closes picking request
    /// </summary>
    /// <param name="pickingRequest">Picking request identifier</param>
    /// <returns>Object with outbound job number and whether outbound is picked</returns>
    [HttpPost("close")]
    public async Task<ClosePickingRequestResponseDto> Close([FromBody] string pickingRequest)
    {
        return await Mediator.Send(new ClosePickingRequestCommand() { PickingRequest = pickingRequest });
    }

    /// <summary>
    /// Returns outbound jobs with all picking request revisions closed
    /// </summary>
    /// <param name="loadingJobNo">Loading number</param>
    /// <returns>List of picking request identifiers</returns>
    [HttpGet("incompleteILogOutbounds")]
    public async Task<List<string>> GetIncompleteILogOutbounds(string loadingJobNo)
    {
        return await Mediator.Send(new GetIncompleteILogOutboundsQuery
        {
            LoadingJobNo = loadingJobNo
        });
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// Base controller 
/// </summary>
[ApiController]
[Route("api/new-api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender mediator = null!;

    /// <summary>
    /// Mediator - it is used to communicate with Application methods 
    /// </summary>
    protected ISender Mediator => mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}

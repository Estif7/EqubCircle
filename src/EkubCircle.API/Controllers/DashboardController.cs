using EkubCircle.Application.DTOs.Dashboard;
using EkubCircle.Application.Features.Dashboard.Queries.GetUserDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EkubCircle.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(UserDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserDashboard(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserDashboardQuery(), cancellationToken);
        return Ok(result);
    }
}

using EkubCircle.Application.DTOs.Rounds;
using EkubCircle.Application.Features.Rounds.Queries.GetCircleRounds;
using EkubCircle.Application.Features.Rounds.Queries.GetCurrentRound;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EkubCircle.API.Controllers;

[ApiController]
[Route("api/circles/{circleId}/[controller]")]
public class RoundsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoundsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RoundDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCircleRounds(Guid circleId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCircleRoundsQuery(circleId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("current")]
    [ProducesResponseType(typeof(RoundDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentRound(Guid circleId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCurrentRoundQuery(circleId), cancellationToken);
        if (result == null)
        {
            return NotFound(new { success = false, message = "No active round found for this circle." });
        }

        return Ok(result);
    }
}

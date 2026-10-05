using EkubCircle.Application.DTOs.Payouts;
using EkubCircle.Application.Features.Payouts.Commands.ExecutePayout;
using EkubCircle.Application.Features.Payouts.Queries.GetCirclePayouts;
using EkubCircle.Application.Features.Payouts.Queries.GetRoundPayout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EkubCircle.API.Controllers;

[ApiController]
public class PayoutsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PayoutsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("api/rounds/{roundId}/payout")]
    [Authorize]
    [ProducesResponseType(typeof(PayoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ExecutePayout(Guid roundId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ExecutePayoutCommand(roundId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("api/rounds/{roundId}/payout")]
    [ProducesResponseType(typeof(PayoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoundPayout(Guid roundId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRoundPayoutQuery(roundId), cancellationToken);
        if (result == null)
        {
            return NotFound(new { success = false, message = "No payout recorded for this round yet." });
        }

        return Ok(result);
    }

    [HttpGet("api/circles/{circleId}/payouts")]
    [ProducesResponseType(typeof(List<PayoutDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCirclePayouts(Guid circleId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCirclePayoutsQuery(circleId), cancellationToken);
        return Ok(result);
    }
}

using EkubCircle.Application.DTOs.Payments;
using EkubCircle.Application.Features.Payments.Commands.RecordPayment;
using EkubCircle.Application.Features.Payments.Queries.GetRoundPaymentProgress;
using EkubCircle.Application.Features.Payments.Queries.GetRoundPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EkubCircle.API.Controllers;

[ApiController]
[Route("api/rounds/{roundId}/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RecordPayment(
        Guid roundId,
        [FromBody] RecordPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RecordPaymentCommand(roundId, request), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayments(Guid roundId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRoundPaymentsQuery(roundId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("progress")]
    [ProducesResponseType(typeof(RoundPaymentProgressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProgress(Guid roundId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRoundPaymentProgressQuery(roundId), cancellationToken);
        return Ok(result);
    }
}

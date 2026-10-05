using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Application.Features.Circles.Commands.CreateCircle;
using EkubCircle.Application.Features.Circles.Commands.JoinCircle;
using EkubCircle.Application.Features.Circles.Commands.StartCircle;
using EkubCircle.Application.Features.Circles.Queries.GetAvailableCircles;
using EkubCircle.Application.Features.Circles.Queries.GetCircleDetails;
using EkubCircle.Application.Features.Circles.Queries.GetMyCircles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EkubCircle.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CirclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CirclesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CircleDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateCircleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCircleCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CircleSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailable(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAvailableCirclesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(List<CircleSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyCircles(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyCirclesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CircleDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCircleDetailsQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/join")]
    [Authorize]
    [ProducesResponseType(typeof(CircleMemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Join(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new JoinCircleCommand(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/start")]
    [Authorize]
    [ProducesResponseType(typeof(CircleDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Start(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new StartCircleCommand(id), cancellationToken);
        return Ok(result);
    }
}

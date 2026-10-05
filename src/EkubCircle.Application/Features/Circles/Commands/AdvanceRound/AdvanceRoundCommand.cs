using EkubCircle.Application.DTOs.Circles;
using MediatR;

namespace EkubCircle.Application.Features.Circles.Commands.AdvanceRound;

public record AdvanceRoundCommand(Guid CircleId) : IRequest<AdvanceRoundResultDto>;

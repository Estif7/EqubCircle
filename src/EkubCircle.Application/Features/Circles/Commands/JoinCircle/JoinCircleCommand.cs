using EkubCircle.Application.DTOs.Circles;
using MediatR;

namespace EkubCircle.Application.Features.Circles.Commands.JoinCircle;

public record JoinCircleCommand(Guid CircleId) : IRequest<CircleMemberDto>;

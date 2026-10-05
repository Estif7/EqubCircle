using EkubCircle.Application.DTOs.Circles;
using MediatR;

namespace EkubCircle.Application.Features.Circles.Commands.StartCircle;

public record StartCircleCommand(Guid CircleId) : IRequest<CircleDetailsDto>;

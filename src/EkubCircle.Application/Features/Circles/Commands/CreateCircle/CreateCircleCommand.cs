using EkubCircle.Application.DTOs.Circles;
using MediatR;

namespace EkubCircle.Application.Features.Circles.Commands.CreateCircle;

public record CreateCircleCommand(CreateCircleRequest Request) : IRequest<CircleDetailsDto>;

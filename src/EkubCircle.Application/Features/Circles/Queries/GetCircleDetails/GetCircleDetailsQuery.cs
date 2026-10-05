using EkubCircle.Application.DTOs.Circles;
using MediatR;

namespace EkubCircle.Application.Features.Circles.Queries.GetCircleDetails;

public record GetCircleDetailsQuery(Guid Id) : IRequest<CircleDetailsDto>;

using EkubCircle.Application.DTOs.Circles;
using MediatR;

namespace EkubCircle.Application.Features.Circles.Queries.GetAvailableCircles;

public record GetAvailableCirclesQuery : IRequest<List<CircleSummaryDto>>;

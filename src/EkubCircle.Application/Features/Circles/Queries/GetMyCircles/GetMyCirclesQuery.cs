using EkubCircle.Application.DTOs.Circles;
using MediatR;

namespace EkubCircle.Application.Features.Circles.Queries.GetMyCircles;

public record GetMyCirclesQuery : IRequest<List<CircleSummaryDto>>;

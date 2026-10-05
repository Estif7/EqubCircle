using EkubCircle.Application.DTOs.Dashboard;
using MediatR;

namespace EkubCircle.Application.Features.Circles.Queries.GetCircleDashboard;

public record GetCircleDashboardQuery(Guid CircleId) : IRequest<CircleDashboardDto>;

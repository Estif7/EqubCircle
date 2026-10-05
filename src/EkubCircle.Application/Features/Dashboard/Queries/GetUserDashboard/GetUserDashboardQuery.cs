using EkubCircle.Application.DTOs.Dashboard;
using MediatR;

namespace EkubCircle.Application.Features.Dashboard.Queries.GetUserDashboard;

public record GetUserDashboardQuery : IRequest<UserDashboardDto>;

using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Circles.Queries.GetAvailableCircles;

public class GetAvailableCirclesQueryHandler : IRequestHandler<GetAvailableCirclesQuery, List<CircleSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetAvailableCirclesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<CircleSummaryDto>> Handle(GetAvailableCirclesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var circles = await _context.Circles
            .AsNoTracking()
            .Include(c => c.Organizer)
            .Include(c => c.Memberships)
            .Where(c => c.Status == CircleStatus.OPEN)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return circles.Select(c => new CircleSummaryDto
        {
            Id = c.Id,
            Name = c.Name,
            ContributionAmount = c.ContributionAmount,
            Frequency = c.Frequency.ToString(),
            MemberLimit = c.MemberLimit,
            MemberCount = c.Memberships.Count,
            Status = c.Status.ToString(),
            OrganizerId = c.OrganizerId,
            OrganizerName = c.Organizer.FullName,
            IsOrganizer = !string.IsNullOrEmpty(currentUserId) && c.OrganizerId == currentUserId,
            IsMember = !string.IsNullOrEmpty(currentUserId) && c.Memberships.Any(m => m.UserId == currentUserId),
            CreatedAt = c.CreatedAt
        }).ToList();
    }
}

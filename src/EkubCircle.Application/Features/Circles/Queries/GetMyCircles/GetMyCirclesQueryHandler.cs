using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Circles.Queries.GetMyCircles;

public class GetMyCirclesQueryHandler : IRequestHandler<GetMyCirclesQuery, List<CircleSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyCirclesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<CircleSummaryDto>> Handle(GetMyCirclesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var circles = await _context.Circles
            .AsNoTracking()
            .Include(c => c.Organizer)
            .Include(c => c.Memberships)
            .Where(c => c.OrganizerId == currentUserId || c.Memberships.Any(m => m.UserId == currentUserId))
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
            IsOrganizer = c.OrganizerId == currentUserId,
            IsMember = c.Memberships.Any(m => m.UserId == currentUserId),
            CreatedAt = c.CreatedAt
        }).ToList();
    }
}

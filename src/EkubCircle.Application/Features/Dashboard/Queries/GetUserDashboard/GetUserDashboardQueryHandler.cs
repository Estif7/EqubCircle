using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Application.DTOs.Dashboard;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Dashboard.Queries.GetUserDashboard;

public class GetUserDashboardQueryHandler : IRequestHandler<GetUserDashboardQuery, UserDashboardDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserDashboardQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<UserDashboardDto> Handle(GetUserDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException("You must be logged in to view your dashboard.");
        }

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User", currentUserId);
        }

        // Fetch all memberships for this user
        var memberships = await _context.CircleMemberships
            .AsNoTracking()
            .Include(m => m.Circle)
                .ThenInclude(c => c.Organizer)
            .Where(m => m.UserId == currentUserId && m.Status == MembershipStatus.ACTIVE)
            .ToListAsync(cancellationToken);

        var membershipIds = memberships.Select(m => m.Id).ToList();

        // Total contributed
        var totalContributed = await _context.Payments
            .AsNoTracking()
            .Where(p => membershipIds.Contains(p.MembershipId))
            .SumAsync(p => p.Amount, cancellationToken);

        // Total received
        var totalReceived = await _context.Payouts
            .AsNoTracking()
            .Where(p => membershipIds.Contains(p.ReceiverMembershipId))
            .SumAsync(p => p.Amount, cancellationToken);

        // Organized circles count
        var organizedCirclesCount = await _context.Circles
            .AsNoTracking()
            .CountAsync(c => c.OrganizerId == currentUserId, cancellationToken);

        // Active circles member counts
        var activeCircleIds = memberships
            .Select(m => m.CircleId)
            .Distinct()
            .ToList();

        var memberCounts = await _context.CircleMemberships
            .AsNoTracking()
            .Where(m => activeCircleIds.Contains(m.CircleId) && m.Status == MembershipStatus.ACTIVE)
            .GroupBy(m => m.CircleId)
            .Select(g => new { CircleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CircleId, x => x.Count, cancellationToken);

        // Active circles summaries
        var activeCircles = memberships
            .Select(m => m.Circle)
            .DistinctBy(c => c.Id)
            .Where(c => c.Status == CircleStatus.ACTIVE || c.Status == CircleStatus.OPEN)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CircleSummaryDto
            {
                Id = c.Id,
                Name = c.Name,
                ContributionAmount = c.ContributionAmount,
                Frequency = c.Frequency.ToString(),
                MemberLimit = c.MemberLimit,
                MemberCount = memberCounts.GetValueOrDefault(c.Id, 0),
                Status = c.Status.ToString(),
                OrganizerId = c.OrganizerId,
                OrganizerName = c.Organizer.FullName,
                IsOrganizer = c.OrganizerId == currentUserId,
                IsMember = true,
                CreatedAt = c.CreatedAt
            })
            .ToList();

        // Pending payments in OPEN rounds
        var activeStatusCircleIds = memberships
            .Where(m => m.Circle.Status == CircleStatus.ACTIVE)
            .Select(m => m.CircleId)
            .ToList();

        var openRounds = await _context.Rounds
            .AsNoTracking()
            .Include(r => r.Circle)
            .Where(r => activeStatusCircleIds.Contains(r.CircleId) && r.Status == RoundStatus.OPEN)
            .ToListAsync(cancellationToken);

        var pendingPayments = new List<UserPendingPaymentDto>();

        foreach (var round in openRounds)
        {
            var userMem = memberships.FirstOrDefault(m => m.CircleId == round.CircleId);
            if (userMem == null) continue;

            var hasPaid = await _context.Payments
                .AsNoTracking()
                .AnyAsync(p => p.RoundId == round.Id && p.MembershipId == userMem.Id, cancellationToken);

            if (!hasPaid)
            {
                pendingPayments.Add(new UserPendingPaymentDto
                {
                    CircleId = round.CircleId,
                    CircleName = round.Circle.Name,
                    RoundId = round.Id,
                    RoundNumber = round.RoundNumber,
                    MembershipId = userMem.Id,
                    AmountDue = round.Circle.ContributionAmount
                });
            }
        }

        return new UserDashboardDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            OrganizedCirclesCount = organizedCirclesCount,
            JoinedCirclesCount = memberships.Count,
            TotalContributed = totalContributed,
            TotalReceived = totalReceived,
            PendingPaymentsCount = pendingPayments.Count,
            ActiveCircles = activeCircles,
            PendingPayments = pendingPayments
        };
    }
}

using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Application.DTOs.Dashboard;
using EkubCircle.Application.DTOs.Payments;
using EkubCircle.Application.DTOs.Payouts;
using EkubCircle.Application.DTOs.Rounds;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Circles.Queries.GetCircleDashboard;

public class GetCircleDashboardQueryHandler : IRequestHandler<GetCircleDashboardQuery, CircleDashboardDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCircleDashboardQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CircleDashboardDto> Handle(GetCircleDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var circle = await _context.Circles
            .AsNoTracking()
            .Include(c => c.Organizer)
            .Include(c => c.Memberships)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == request.CircleId, cancellationToken);

        if (circle == null)
        {
            throw new NotFoundException("Circle", request.CircleId);
        }

        var activeMembers = circle.Memberships
            .Where(m => m.Status == MembershipStatus.ACTIVE)
            .OrderBy(m => m.PayoutOrder)
            .ToList();

        var circleDetailsDto = new CircleDetailsDto
        {
            Id = circle.Id,
            Name = circle.Name,
            ContributionAmount = circle.ContributionAmount,
            Frequency = circle.Frequency.ToString(),
            MemberLimit = circle.MemberLimit,
            MemberCount = activeMembers.Count,
            Status = circle.Status.ToString(),
            OrganizerId = circle.OrganizerId,
            OrganizerName = circle.Organizer.FullName,
            IsOrganizer = !string.IsNullOrEmpty(currentUserId) && circle.OrganizerId == currentUserId,
            IsMember = !string.IsNullOrEmpty(currentUserId) && activeMembers.Any(m => m.UserId == currentUserId),
            CreatedAt = circle.CreatedAt,
            StartedAt = circle.StartedAt,
            CompletedAt = circle.CompletedAt,
            Members = activeMembers.Select(m => new CircleMemberDto
            {
                MembershipId = m.Id,
                UserId = m.UserId,
                FullName = m.User.FullName,
                Email = m.User.Email ?? string.Empty,
                PhoneNumber = m.User.PhoneNumber ?? string.Empty,
                RoleInCircle = m.RoleInCircle.ToString(),
                PayoutOrder = m.PayoutOrder,
                Status = m.Status.ToString(),
                JoinedAt = m.JoinedAt
            }).ToList()
        };

        var expectedPot = circle.ContributionAmount * activeMembers.Count;

        // Fetch all rounds
        var rounds = await _context.Rounds
            .AsNoTracking()
            .Include(r => r.ReceiverMembership)
                .ThenInclude(m => m.User)
            .Where(r => r.CircleId == request.CircleId)
            .OrderBy(r => r.RoundNumber)
            .ToListAsync(cancellationToken);

        var roundDtos = rounds.Select(r => new RoundDto
        {
            Id = r.Id,
            CircleId = r.CircleId,
            RoundNumber = r.RoundNumber,
            ReceiverMembershipId = r.ReceiverMembershipId,
            ReceiverUserId = r.ReceiverMembership.UserId,
            ReceiverName = r.ReceiverMembership.User.FullName,
            Status = r.Status.ToString(),
            ExpectedPayoutAmount = expectedPot,
            PayoutAmount = r.PayoutAmount,
            OpenedAt = r.OpenedAt,
            PaidOutAt = r.PaidOutAt,
            IsCurrentRound = r.Status == RoundStatus.OPEN
        }).ToList();

        // Find active round
        var activeRound = rounds.FirstOrDefault(r => r.Status == RoundStatus.OPEN || r.Status == RoundStatus.PAID_OUT);
        RoundPaymentProgressDto? progressDto = null;

        if (activeRound != null)
        {
            var roundPayments = await _context.Payments
                .AsNoTracking()
                .Where(p => p.RoundId == activeRound.Id)
                .ToListAsync(cancellationToken);

            var paidMembershipIds = roundPayments.Select(p => p.MembershipId).ToHashSet();
            var totalCollected = roundPayments.Sum(p => p.Amount);
            var paidCount = roundPayments.Count;
            var unpaidCount = activeMembers.Count - paidCount;

            progressDto = new RoundPaymentProgressDto
            {
                RoundId = activeRound.Id,
                RoundNumber = activeRound.RoundNumber,
                CircleId = circle.Id,
                CircleName = circle.Name,
                ContributionAmount = circle.ContributionAmount,
                ExpectedTotalAmount = expectedPot,
                CollectedAmount = totalCollected,
                TotalMembers = activeMembers.Count,
                PaidCount = paidCount,
                UnpaidCount = Math.Max(0, unpaidCount),
                IsFullyPaid = unpaidCount <= 0 && totalCollected >= expectedPot,
                PaidMembers = circleDetailsDto.Members.Where(m => paidMembershipIds.Contains(m.MembershipId)).ToList(),
                UnpaidMembers = circleDetailsDto.Members.Where(m => !paidMembershipIds.Contains(m.MembershipId)).ToList()
            };
        }

        // Fetch payouts
        var payouts = await _context.Payouts
            .AsNoTracking()
            .Include(p => p.Round)
            .Include(p => p.ReceiverMembership)
                .ThenInclude(m => m.User)
            .Include(p => p.ConfirmedByUser)
            .Where(p => p.Round.CircleId == request.CircleId)
            .OrderBy(p => p.Round.RoundNumber)
            .ToListAsync(cancellationToken);

        var payoutDtos = payouts.Select(p => new PayoutDto
        {
            Id = p.Id,
            RoundId = p.RoundId,
            RoundNumber = p.Round.RoundNumber,
            CircleId = circle.Id,
            CircleName = circle.Name,
            ReceiverMembershipId = p.ReceiverMembershipId,
            ReceiverUserId = p.ReceiverMembership.UserId,
            ReceiverName = p.ReceiverMembership.User.FullName,
            Amount = p.Amount,
            PaidAt = p.PaidAt,
            ConfirmedByUserId = p.ConfirmedByUserId,
            ConfirmedByName = p.ConfirmedByUser.FullName,
            Status = p.Status.ToString()
        }).ToList();

        var completedRoundsCount = rounds.Count(r => r.Status == RoundStatus.COMPLETED || r.Status == RoundStatus.PAID_OUT);
        var totalDisbursed = payouts.Sum(p => p.Amount);
        var totalExpected = expectedPot * rounds.Count;

        return new CircleDashboardDto
        {
            Circle = circleDetailsDto,
            ActiveRound = activeRound != null ? roundDtos.FirstOrDefault(r => r.Id == activeRound.Id) : null,
            ActiveRoundProgress = progressDto,
            Rounds = roundDtos,
            Payouts = payoutDtos,
            TotalRounds = rounds.Count,
            CompletedRounds = completedRoundsCount,
            TotalDisbursed = totalDisbursed,
            TotalExpected = totalExpected
        };
    }
}

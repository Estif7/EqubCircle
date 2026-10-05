using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Application.DTOs.Payments;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Payments.Queries.GetRoundPaymentProgress;

public class GetRoundPaymentProgressQueryHandler : IRequestHandler<GetRoundPaymentProgressQuery, RoundPaymentProgressDto>
{
    private readonly IApplicationDbContext _context;

    public GetRoundPaymentProgressQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoundPaymentProgressDto> Handle(GetRoundPaymentProgressQuery request, CancellationToken cancellationToken)
    {
        var round = await _context.Rounds
            .AsNoTracking()
            .Include(r => r.Circle)
                .ThenInclude(c => c.Memberships)
                    .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(r => r.Id == request.RoundId, cancellationToken);

        if (round == null)
        {
            throw new NotFoundException("Round", request.RoundId);
        }

        var activeMembers = round.Circle.Memberships
            .Where(m => m.Status == MembershipStatus.ACTIVE)
            .OrderBy(m => m.PayoutOrder ?? int.MaxValue)
            .ToList();

        var payments = await _context.Payments
            .AsNoTracking()
            .Where(p => p.RoundId == request.RoundId)
            .ToListAsync(cancellationToken);

        var paidMembershipIds = payments.Select(p => p.MembershipId).ToHashSet();

        var paidMembers = activeMembers
            .Where(m => paidMembershipIds.Contains(m.Id))
            .Select(m => new CircleMemberDto
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
            }).ToList();

        var unpaidMembers = activeMembers
            .Where(m => !paidMembershipIds.Contains(m.Id))
            .Select(m => new CircleMemberDto
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
            }).ToList();

        var expectedPot = round.Circle.ContributionAmount * activeMembers.Count;
        var collectedPot = payments.Sum(p => p.Amount);

        return new RoundPaymentProgressDto
        {
            RoundId = round.Id,
            RoundNumber = round.RoundNumber,
            CircleId = round.CircleId,
            CircleName = round.Circle.Name,
            ContributionAmount = round.Circle.ContributionAmount,
            ExpectedTotalAmount = expectedPot,
            CollectedAmount = collectedPot,
            TotalMembers = activeMembers.Count,
            PaidCount = paidMembers.Count,
            UnpaidCount = unpaidMembers.Count,
            IsFullyPaid = unpaidMembers.Count == 0 && activeMembers.Count > 0,
            PaidMembers = paidMembers,
            UnpaidMembers = unpaidMembers
        };
    }
}

using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Rounds;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Rounds.Queries.GetCircleRounds;

public class GetCircleRoundsQueryHandler : IRequestHandler<GetCircleRoundsQuery, List<RoundDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCircleRoundsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoundDto>> Handle(GetCircleRoundsQuery request, CancellationToken cancellationToken)
    {
        var circle = await _context.Circles
            .AsNoTracking()
            .Include(c => c.Memberships)
            .FirstOrDefaultAsync(c => c.Id == request.CircleId, cancellationToken);

        if (circle == null)
        {
            throw new NotFoundException("Circle", request.CircleId);
        }

        var activeMemberCount = circle.Memberships.Count(m => m.Status == MembershipStatus.ACTIVE);
        var expectedPot = circle.ContributionAmount * activeMemberCount;

        var rounds = await _context.Rounds
            .AsNoTracking()
            .Include(r => r.ReceiverMembership)
                .ThenInclude(m => m.User)
            .Where(r => r.CircleId == request.CircleId)
            .OrderBy(r => r.RoundNumber)
            .ToListAsync(cancellationToken);

        return rounds.Select(r => new RoundDto
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
    }
}

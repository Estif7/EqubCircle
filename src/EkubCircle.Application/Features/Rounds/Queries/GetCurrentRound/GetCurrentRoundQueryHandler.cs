using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Rounds;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Rounds.Queries.GetCurrentRound;

public class GetCurrentRoundQueryHandler : IRequestHandler<GetCurrentRoundQuery, RoundDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCurrentRoundQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoundDto?> Handle(GetCurrentRoundQuery request, CancellationToken cancellationToken)
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

        var round = await _context.Rounds
            .AsNoTracking()
            .Include(r => r.ReceiverMembership)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(r => r.CircleId == request.CircleId && r.Status == RoundStatus.OPEN, cancellationToken);

        if (round == null)
        {
            return null;
        }

        return new RoundDto
        {
            Id = round.Id,
            CircleId = round.CircleId,
            RoundNumber = round.RoundNumber,
            ReceiverMembershipId = round.ReceiverMembershipId,
            ReceiverUserId = round.ReceiverMembership.UserId,
            ReceiverName = round.ReceiverMembership.User.FullName,
            Status = round.Status.ToString(),
            ExpectedPayoutAmount = expectedPot,
            PayoutAmount = round.PayoutAmount,
            OpenedAt = round.OpenedAt,
            PaidOutAt = round.PaidOutAt,
            IsCurrentRound = true
        };
    }
}

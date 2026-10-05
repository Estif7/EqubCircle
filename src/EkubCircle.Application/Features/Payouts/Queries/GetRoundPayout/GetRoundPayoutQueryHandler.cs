using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Payouts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Payouts.Queries.GetRoundPayout;

public class GetRoundPayoutQueryHandler : IRequestHandler<GetRoundPayoutQuery, PayoutDto?>
{
    private readonly IApplicationDbContext _context;

    public GetRoundPayoutQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PayoutDto?> Handle(GetRoundPayoutQuery request, CancellationToken cancellationToken)
    {
        var payout = await _context.Payouts
            .AsNoTracking()
            .Include(p => p.Round)
                .ThenInclude(r => r.Circle)
            .Include(p => p.ReceiverMembership)
                .ThenInclude(m => m.User)
            .Include(p => p.ConfirmedByUser)
            .FirstOrDefaultAsync(p => p.RoundId == request.RoundId, cancellationToken);

        if (payout == null)
        {
            return null;
        }

        return new PayoutDto
        {
            Id = payout.Id,
            RoundId = payout.RoundId,
            RoundNumber = payout.Round.RoundNumber,
            CircleId = payout.Round.CircleId,
            CircleName = payout.Round.Circle.Name,
            ReceiverMembershipId = payout.ReceiverMembershipId,
            ReceiverUserId = payout.ReceiverMembership.UserId,
            ReceiverName = payout.ReceiverMembership.User.FullName,
            Amount = payout.Amount,
            PaidAt = payout.PaidAt,
            ConfirmedByUserId = payout.ConfirmedByUserId,
            ConfirmedByName = payout.ConfirmedByUser.FullName,
            Status = payout.Status.ToString()
        };
    }
}

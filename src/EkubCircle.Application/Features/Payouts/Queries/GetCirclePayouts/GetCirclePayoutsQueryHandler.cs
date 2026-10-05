using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Payouts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Payouts.Queries.GetCirclePayouts;

public class GetCirclePayoutsQueryHandler : IRequestHandler<GetCirclePayoutsQuery, List<PayoutDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCirclePayoutsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PayoutDto>> Handle(GetCirclePayoutsQuery request, CancellationToken cancellationToken)
    {
        var circle = await _context.Circles
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CircleId, cancellationToken);

        if (circle == null)
        {
            throw new NotFoundException("Circle", request.CircleId);
        }

        var payouts = await _context.Payouts
            .AsNoTracking()
            .Include(p => p.Round)
                .ThenInclude(r => r.Circle)
            .Include(p => p.ReceiverMembership)
                .ThenInclude(m => m.User)
            .Include(p => p.ConfirmedByUser)
            .Where(p => p.Round.CircleId == request.CircleId)
            .OrderBy(p => p.Round.RoundNumber)
            .ToListAsync(cancellationToken);

        return payouts.Select(p => new PayoutDto
        {
            Id = p.Id,
            RoundId = p.RoundId,
            RoundNumber = p.Round.RoundNumber,
            CircleId = p.Round.CircleId,
            CircleName = p.Round.Circle.Name,
            ReceiverMembershipId = p.ReceiverMembershipId,
            ReceiverUserId = p.ReceiverMembership.UserId,
            ReceiverName = p.ReceiverMembership.User.FullName,
            Amount = p.Amount,
            PaidAt = p.PaidAt,
            ConfirmedByUserId = p.ConfirmedByUserId,
            ConfirmedByName = p.ConfirmedByUser.FullName,
            Status = p.Status.ToString()
        }).ToList();
    }
}

using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Payments.Queries.GetRoundPayments;

public class GetRoundPaymentsQueryHandler : IRequestHandler<GetRoundPaymentsQuery, List<PaymentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRoundPaymentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentDto>> Handle(GetRoundPaymentsQuery request, CancellationToken cancellationToken)
    {
        var round = await _context.Rounds
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RoundId, cancellationToken);

        if (round == null)
        {
            throw new NotFoundException("Round", request.RoundId);
        }

        var payments = await _context.Payments
            .AsNoTracking()
            .Include(p => p.Membership)
                .ThenInclude(m => m.User)
            .Include(p => p.RecordedByUser)
            .Where(p => p.RoundId == request.RoundId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync(cancellationToken);

        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            RoundId = p.RoundId,
            RoundNumber = round.RoundNumber,
            MembershipId = p.MembershipId,
            MemberUserId = p.Membership.UserId,
            MemberName = p.Membership.User.FullName,
            MemberEmail = p.Membership.User.Email ?? string.Empty,
            Amount = p.Amount,
            PaidAt = p.PaidAt,
            Reference = p.Reference,
            Note = p.Note,
            RecordedByUserId = p.RecordedByUserId,
            RecordedByName = p.RecordedByUser.FullName
        }).ToList();
    }
}

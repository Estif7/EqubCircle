using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Payouts;
using EkubCircle.Domain.Entities;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Payouts.Commands.ExecutePayout;

public class ExecutePayoutCommandHandler : IRequestHandler<ExecutePayoutCommand, PayoutDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExecutePayoutCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<PayoutDto> Handle(ExecutePayoutCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var currentUser = await _userManager.FindByIdAsync(currentUserId);
        if (currentUser == null)
        {
            throw new NotFoundException("User", currentUserId);
        }

        // Section 22: The operation must be transactional
        using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        var round = await _context.Rounds
            .Include(r => r.Circle)
                .ThenInclude(c => c.Memberships)
                    .ThenInclude(m => m.User)
            .Include(r => r.ReceiverMembership)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(r => r.Id == command.RoundId, cancellationToken);

        if (round == null)
        {
            throw new NotFoundException("Round", command.RoundId);
        }

        // Verify organizer
        if (round.Circle.OrganizerId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the circle organizer can execute payouts.");
        }

        // Verify round is open
        if (round.Status != RoundStatus.OPEN)
        {
            throw new ConflictException($"Payout cannot be executed because round status is {round.Status}. Only OPEN rounds can be paid out.");
        }

        // Load all active members
        var activeMembers = round.Circle.Memberships
            .Where(m => m.Status == MembershipStatus.ACTIVE)
            .ToList();

        if (activeMembers.Count == 0)
        {
            throw new ConflictException("No active members found in this circle.");
        }

        // Load all persisted payments for this round
        var payments = await _context.Payments
            .Where(p => p.RoundId == round.Id)
            .ToListAsync(cancellationToken);

        var paidMembershipIds = payments.Select(p => p.MembershipId).ToHashSet();
        var unpaidMembers = activeMembers.Where(m => !paidMembershipIds.Contains(m.Id)).ToList();

        // Verify EVERY active member has paid
        if (unpaidMembers.Any())
        {
            var unpaidNames = string.Join(", ", unpaidMembers.Select(u => u.User.FullName));
            throw new ConflictException($"All members must pay before the payout can be executed. Pending payments from: {unpaidNames}.");
        }

        // Determine receiver from payout order
        var receiverMembership = round.ReceiverMembership;
        if (receiverMembership == null)
        {
            throw new InvalidOperationException("Receiver membership could not be resolved for this round.");
        }

        // Verify receiver has not received before in this circle
        var hasReceivedBefore = await _context.Payouts
            .Include(p => p.Round)
            .AnyAsync(p => p.Round.CircleId == round.CircleId &&
                           p.ReceiverMembershipId == receiverMembership.Id &&
                           p.Status == PayoutStatus.COMPLETED,
                      cancellationToken);

        if (hasReceivedBefore)
        {
            throw new ConflictException($"Member '{receiverMembership.User.FullName}' has already received a payout in this circle.");
        }

        // Calculate pot from persisted payments
        var potAmount = payments.Sum(p => p.Amount);

        // Create payout record
        var payout = new Payout
        {
            RoundId = round.Id,
            ReceiverMembershipId = receiverMembership.Id,
            Amount = potAmount,
            PaidAt = DateTime.UtcNow,
            ConfirmedByUserId = currentUserId,
            Status = PayoutStatus.COMPLETED
        };

        _context.Payouts.Add(payout);

        // Mark round paid out
        round.Status = RoundStatus.PAID_OUT;
        round.PaidOutAt = DateTime.UtcNow;
        round.PayoutAmount = potAmount;

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new PayoutDto
        {
            Id = payout.Id,
            RoundId = round.Id,
            RoundNumber = round.RoundNumber,
            CircleId = round.CircleId,
            CircleName = round.Circle.Name,
            ReceiverMembershipId = receiverMembership.Id,
            ReceiverUserId = receiverMembership.UserId,
            ReceiverName = receiverMembership.User.FullName,
            Amount = payout.Amount,
            PaidAt = payout.PaidAt,
            ConfirmedByUserId = currentUserId,
            ConfirmedByName = currentUser.FullName,
            Status = payout.Status.ToString()
        };
    }
}

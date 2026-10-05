using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Application.DTOs.Rounds;
using EkubCircle.Domain.Entities;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Circles.Commands.AdvanceRound;

public class AdvanceRoundCommandHandler : IRequestHandler<AdvanceRoundCommand, AdvanceRoundResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AdvanceRoundCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<AdvanceRoundResultDto> Handle(AdvanceRoundCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException("You must be logged in to advance a round.");
        }

        var circle = await _context.Circles
            .Include(c => c.Memberships)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == request.CircleId, cancellationToken);

        if (circle == null)
        {
            throw new NotFoundException("Circle", request.CircleId);
        }

        if (circle.OrganizerId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the circle organizer can advance rounds.");
        }

        if (circle.Status == CircleStatus.COMPLETED)
        {
            throw new ConflictException("Circle is already completed.");
        }

        if (circle.Status != CircleStatus.ACTIVE)
        {
            throw new ConflictException($"Circle cannot advance because it is in '{circle.Status}' status. It must be ACTIVE.");
        }

        var rounds = await _context.Rounds
            .Include(r => r.ReceiverMembership)
                .ThenInclude(m => m.User)
            .Where(r => r.CircleId == request.CircleId)
            .OrderBy(r => r.RoundNumber)
            .ToListAsync(cancellationToken);

        if (!rounds.Any())
        {
            throw new ConflictException("No rounds exist for this circle. The circle has not been initialized properly.");
        }

        // Check if there is currently an OPEN round that hasn't been paid out
        var openRound = rounds.FirstOrDefault(r => r.Status == RoundStatus.OPEN);
        if (openRound != null)
        {
            throw new ConflictException($"Round {openRound.RoundNumber} is still OPEN. You must execute its payout before advancing to the next round.");
        }

        // Find the round with PAID_OUT status waiting to be completed and advanced
        var paidOutRound = rounds.FirstOrDefault(r => r.Status == RoundStatus.PAID_OUT);
        if (paidOutRound == null)
        {
            throw new ConflictException("There is no completed payout waiting for advancement.");
        }

        // Mark current round as COMPLETED
        paidOutRound.Status = RoundStatus.COMPLETED;

        // Check if there is a next round
        var nextRound = rounds.FirstOrDefault(r => r.RoundNumber == paidOutRound.RoundNumber + 1);
        bool isCircleCompleted = false;
        string message;

        if (nextRound != null)
        {
            nextRound.Status = RoundStatus.OPEN;
            nextRound.OpenedAt = DateTime.UtcNow;
            message = $"Round {paidOutRound.RoundNumber} completed. Round {nextRound.RoundNumber} is now active and open for contributions.";
        }
        else
        {
            // All rounds concluded!
            circle.Status = CircleStatus.COMPLETED;
            circle.CompletedAt = DateTime.UtcNow;
            isCircleCompleted = true;
            message = $"All {rounds.Count} rounds have been successfully completed and paid out. The circle is now fully completed!";
        }

        await _context.SaveChangesAsync(cancellationToken);

        var activeMemberCount = circle.Memberships.Count(m => m.Status == MembershipStatus.ACTIVE);
        var expectedPot = circle.ContributionAmount * activeMemberCount;

        return new AdvanceRoundResultDto
        {
            CircleId = circle.Id,
            CircleName = circle.Name,
            CircleStatus = circle.Status.ToString(),
            IsCircleCompleted = isCircleCompleted,
            CircleCompletedAt = circle.CompletedAt,
            PreviousRound = MapToDto(paidOutRound, expectedPot),
            CurrentRound = nextRound != null ? MapToDto(nextRound, expectedPot) : null,
            Message = message
        };
    }

    private static RoundDto MapToDto(Round r, decimal expectedPot)
    {
        return new RoundDto
        {
            Id = r.Id,
            CircleId = r.CircleId,
            RoundNumber = r.RoundNumber,
            ReceiverMembershipId = r.ReceiverMembershipId,
            ReceiverUserId = r.ReceiverMembership?.UserId ?? string.Empty,
            ReceiverName = r.ReceiverMembership?.User?.FullName ?? string.Empty,
            Status = r.Status.ToString(),
            ExpectedPayoutAmount = expectedPot,
            PayoutAmount = r.PayoutAmount,
            OpenedAt = r.OpenedAt,
            PaidOutAt = r.PaidOutAt,
            IsCurrentRound = r.Status == RoundStatus.OPEN
        };
    }
}

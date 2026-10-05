using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Payments;
using EkubCircle.Domain.Entities;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Payments.Commands.RecordPayment;

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, PaymentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecordPaymentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<PaymentDto> Handle(RecordPaymentCommand command, CancellationToken cancellationToken)
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

        var round = await _context.Rounds
            .Include(r => r.Circle)
            .FirstOrDefaultAsync(r => r.Id == command.RoundId, cancellationToken);

        if (round == null)
        {
            throw new NotFoundException("Round", command.RoundId);
        }

        if (round.Circle.OrganizerId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the circle organizer can record member payments.");
        }

        if (round.Status != RoundStatus.OPEN)
        {
            throw new ConflictException($"Payments can only be recorded for OPEN rounds. Current status is {round.Status}.");
        }

        var membership = await _context.CircleMemberships
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == command.Request.MembershipId, cancellationToken);

        if (membership == null)
        {
            throw new NotFoundException("Membership", command.Request.MembershipId);
        }

        if (membership.CircleId != round.CircleId)
        {
            throw new ValidationException("Selected member does not belong to the circle of this round.");
        }

        if (membership.Status != MembershipStatus.ACTIVE)
        {
            throw new ConflictException("Cannot record payment for an inactive membership.");
        }

        if (command.Request.Amount != round.Circle.ContributionAmount)
        {
            throw new ValidationException($"Payment amount must exactly equal the circle contribution amount of {round.Circle.ContributionAmount:F2} ETB.");
        }

        // Enforce duplicate protection: One member can have only one payment for a particular round
        var alreadyPaid = await _context.Payments
            .AnyAsync(p => p.RoundId == round.Id && p.MembershipId == membership.Id, cancellationToken);

        if (alreadyPaid)
        {
            throw new ConflictException($"Payment has already been recorded for member '{membership.User.FullName}' in Round #{round.RoundNumber}.");
        }

        var payment = new Payment
        {
            RoundId = round.Id,
            MembershipId = membership.Id,
            Amount = command.Request.Amount,
            PaidAt = DateTime.UtcNow,
            Reference = command.Request.Reference.Trim(),
            Note = command.Request.Note?.Trim(),
            RecordedByUserId = currentUserId
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentDto
        {
            Id = payment.Id,
            RoundId = round.Id,
            RoundNumber = round.RoundNumber,
            MembershipId = membership.Id,
            MemberUserId = membership.UserId,
            MemberName = membership.User.FullName,
            MemberEmail = membership.User.Email ?? string.Empty,
            Amount = payment.Amount,
            PaidAt = payment.PaidAt,
            Reference = payment.Reference,
            Note = payment.Note,
            RecordedByUserId = currentUserId,
            RecordedByName = currentUser.FullName
        };
    }
}

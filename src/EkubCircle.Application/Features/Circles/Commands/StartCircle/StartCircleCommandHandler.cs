using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Domain.Entities;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Circles.Commands.StartCircle;

public class StartCircleCommandHandler : IRequestHandler<StartCircleCommand, CircleDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public StartCircleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CircleDetailsDto> Handle(StartCircleCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var circle = await _context.Circles
            .Include(c => c.Organizer)
            .Include(c => c.Memberships)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == command.CircleId, cancellationToken);

        if (circle == null)
        {
            throw new NotFoundException("Circle", command.CircleId);
        }

        if (circle.OrganizerId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the circle organizer can start the circle.");
        }

        if (circle.Status != CircleStatus.OPEN)
        {
            throw new ConflictException($"Circle cannot be started because its current status is {circle.Status}.");
        }

        var activeMembers = circle.Memberships
            .Where(m => m.Status == MembershipStatus.ACTIVE)
            .OrderBy(m => m.RoleInCircle == CircleRole.ORGANIZER ? 0 : 1)
            .ThenBy(m => m.JoinedAt)
            .ToList();

        if (activeMembers.Count < 2)
        {
            throw new ConflictException("Circle must have at least 2 members before it can be started.");
        }

        // 1. Establish deterministic payout order
        for (int i = 0; i < activeMembers.Count; i++)
        {
            activeMembers[i].PayoutOrder = i + 1;
        }

        // 2. Generate rounds
        for (int i = 0; i < activeMembers.Count; i++)
        {
            var round = new Round
            {
                CircleId = circle.Id,
                RoundNumber = i + 1,
                ReceiverMembershipId = activeMembers[i].Id,
                Status = (i == 0) ? RoundStatus.OPEN : RoundStatus.PENDING,
                OpenedAt = (i == 0) ? DateTime.UtcNow : null,
                PayoutAmount = 0
            };

            _context.Rounds.Add(round);
        }

        // 3. Lock circle & update status to ACTIVE
        circle.Status = CircleStatus.ACTIVE;
        circle.StartedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new CircleDetailsDto
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
            IsOrganizer = true,
            IsMember = true,
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
    }
}

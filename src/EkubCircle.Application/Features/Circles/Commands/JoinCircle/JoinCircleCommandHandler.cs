using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Domain.Entities;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Circles.Commands.JoinCircle;

public class JoinCircleCommandHandler : IRequestHandler<JoinCircleCommand, CircleMemberDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public JoinCircleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<CircleMemberDto> Handle(JoinCircleCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var user = await _userManager.FindByIdAsync(currentUserId);
        if (user == null)
        {
            throw new NotFoundException("User", currentUserId);
        }

        if (!user.FaydaVerified)
        {
            throw new InvalidOperationException("Only Fayda-verified users can join circles.");
        }

        var circle = await _context.Circles
            .Include(c => c.Memberships)
            .FirstOrDefaultAsync(c => c.Id == command.CircleId, cancellationToken);

        if (circle == null)
        {
            throw new NotFoundException("Circle", command.CircleId);
        }

        if (circle.Status != CircleStatus.OPEN)
        {
            throw new ConflictException("Circle is not open for new members.");
        }

        if (circle.Memberships.Count >= circle.MemberLimit)
        {
            throw new ConflictException("Circle has reached its maximum member capacity.");
        }

        if (circle.Memberships.Any(m => m.UserId == user.Id))
        {
            throw new ConflictException("You are already a member of this circle.");
        }

        var membership = new CircleMembership
        {
            CircleId = circle.Id,
            UserId = user.Id,
            RoleInCircle = CircleRole.MEMBER,
            Status = MembershipStatus.ACTIVE,
            JoinedAt = DateTime.UtcNow
        };

        _context.CircleMemberships.Add(membership);
        await _context.SaveChangesAsync(cancellationToken);

        return new CircleMemberDto
        {
            MembershipId = membership.Id,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            RoleInCircle = membership.RoleInCircle.ToString(),
            PayoutOrder = membership.PayoutOrder,
            Status = membership.Status.ToString(),
            JoinedAt = membership.JoinedAt
        };
    }
}

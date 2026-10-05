using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Domain.Entities;
using EkubCircle.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EkubCircle.Application.Features.Circles.Commands.CreateCircle;

public class CreateCircleCommandHandler : IRequestHandler<CreateCircleCommand, CircleDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateCircleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<CircleDetailsDto> Handle(CreateCircleCommand command, CancellationToken cancellationToken)
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
            throw new InvalidOperationException("Only Fayda-verified users can create a circle.");
        }

        var request = command.Request;

        var circle = new Circle
        {
            Name = request.Name.Trim(),
            ContributionAmount = request.ContributionAmount,
            Frequency = request.Frequency,
            MemberLimit = request.MemberLimit,
            Status = CircleStatus.OPEN,
            OrganizerId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        // Section 1: The organizer is automatically a member of the circle they create
        var organizerMembership = new CircleMembership
        {
            CircleId = circle.Id,
            UserId = user.Id,
            RoleInCircle = CircleRole.ORGANIZER,
            Status = MembershipStatus.ACTIVE,
            JoinedAt = DateTime.UtcNow
        };

        circle.Memberships.Add(organizerMembership);

        _context.Circles.Add(circle);
        await _context.SaveChangesAsync(cancellationToken);

        return new CircleDetailsDto
        {
            Id = circle.Id,
            Name = circle.Name,
            ContributionAmount = circle.ContributionAmount,
            Frequency = circle.Frequency.ToString(),
            MemberLimit = circle.MemberLimit,
            MemberCount = 1,
            Status = circle.Status.ToString(),
            OrganizerId = user.Id,
            OrganizerName = user.FullName,
            IsOrganizer = true,
            IsMember = true,
            CreatedAt = circle.CreatedAt,
            StartedAt = circle.StartedAt,
            CompletedAt = circle.CompletedAt,
            Members = new List<CircleMemberDto>
            {
                new()
                {
                    MembershipId = organizerMembership.Id,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    RoleInCircle = organizerMembership.RoleInCircle.ToString(),
                    PayoutOrder = organizerMembership.PayoutOrder,
                    Status = organizerMembership.Status.ToString(),
                    JoinedAt = organizerMembership.JoinedAt
                }
            }
        };
    }
}

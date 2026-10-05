using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Circles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Features.Circles.Queries.GetCircleDetails;

public class GetCircleDetailsQueryHandler : IRequestHandler<GetCircleDetailsQuery, CircleDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCircleDetailsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CircleDetailsDto> Handle(GetCircleDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var circle = await _context.Circles
            .AsNoTracking()
            .Include(c => c.Organizer)
            .Include(c => c.Memberships)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (circle == null)
        {
            throw new NotFoundException("Circle", request.Id);
        }

        return new CircleDetailsDto
        {
            Id = circle.Id,
            Name = circle.Name,
            ContributionAmount = circle.ContributionAmount,
            Frequency = circle.Frequency.ToString(),
            MemberLimit = circle.MemberLimit,
            MemberCount = circle.Memberships.Count,
            Status = circle.Status.ToString(),
            OrganizerId = circle.OrganizerId,
            OrganizerName = circle.Organizer.FullName,
            IsOrganizer = !string.IsNullOrEmpty(currentUserId) && circle.OrganizerId == currentUserId,
            IsMember = !string.IsNullOrEmpty(currentUserId) && circle.Memberships.Any(m => m.UserId == currentUserId),
            CreatedAt = circle.CreatedAt,
            StartedAt = circle.StartedAt,
            CompletedAt = circle.CompletedAt,
            Members = circle.Memberships
                .OrderBy(m => m.JoinedAt)
                .Select(m => new CircleMemberDto
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

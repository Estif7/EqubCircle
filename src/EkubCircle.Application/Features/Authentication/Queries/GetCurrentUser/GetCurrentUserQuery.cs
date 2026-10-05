using EkubCircle.Application.DTOs.Authentication;
using MediatR;

namespace EkubCircle.Application.Features.Authentication.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserDto>;

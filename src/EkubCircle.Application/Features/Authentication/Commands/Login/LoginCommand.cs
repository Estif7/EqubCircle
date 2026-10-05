using EkubCircle.Application.DTOs.Authentication;
using MediatR;

namespace EkubCircle.Application.Features.Authentication.Commands.Login;

public record LoginCommand(LoginRequest Request) : IRequest<AuthResponseDto>;

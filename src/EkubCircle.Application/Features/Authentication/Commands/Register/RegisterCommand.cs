using EkubCircle.Application.DTOs.Authentication;
using MediatR;

namespace EkubCircle.Application.Features.Authentication.Commands.Register;

public record RegisterCommand(RegisterRequest Request) : IRequest<RegisterResponse>;

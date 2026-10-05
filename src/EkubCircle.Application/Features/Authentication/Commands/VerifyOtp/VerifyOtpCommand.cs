using EkubCircle.Application.DTOs.Authentication;
using MediatR;

namespace EkubCircle.Application.Features.Authentication.Commands.VerifyOtp;

public record VerifyOtpCommand(VerifyOtpRequest Request) : IRequest<AuthResponseDto>;

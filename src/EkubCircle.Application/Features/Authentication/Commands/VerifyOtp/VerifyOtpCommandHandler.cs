using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Authentication;
using EkubCircle.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EkubCircle.Application.Features.Authentication.Commands.VerifyOtp;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFaydaVerificationService _faydaVerificationService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public VerifyOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IFaydaVerificationService faydaVerificationService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _faydaVerificationService = faydaVerificationService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(VerifyOtpCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException("User", request.UserId);
        }

        var (success, message) = await _faydaVerificationService.VerifyOtpAsync(
            user.Id,
            request.OtpCode,
            cancellationToken);

        if (!success)
        {
            throw new InvalidOperationException(message);
        }

        user.FaydaVerified = true;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw new InvalidOperationException("Failed to update user verification status.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresIn = 86400,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                FaydaVerified = user.FaydaVerified,
                CreatedAt = user.CreatedAt
            }
        };
    }
}

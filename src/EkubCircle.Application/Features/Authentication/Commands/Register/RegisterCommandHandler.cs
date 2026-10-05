using EkubCircle.Application.Common.Exceptions;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Application.DTOs.Authentication;
using EkubCircle.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EkubCircle.Application.Features.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFaydaVerificationService _faydaVerificationService;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IFaydaVerificationService faydaVerificationService)
    {
        _userManager = userManager;
        _faydaVerificationService = faydaVerificationService;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        // Check if user with this email already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName.Trim(),
            FaydaVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(
                e => e.Code,
                e => new[] { e.Description });
            throw new ValidationException(errors);
        }

        // Simulate Fayda verification request and OTP generation
        var (success, message, simulatedOtp) = await _faydaVerificationService.RequestVerificationAsync(
            user.Id,
            request.FaydaFan,
            request.PhoneNumber,
            cancellationToken);

        if (!success)
        {
            throw new InvalidOperationException($"Fayda verification request failed: {message}");
        }

        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Message = "Registration successful. Please verify the OTP sent to your phone to complete Fayda verification.",
            DevSimulatedOtp = simulatedOtp
        };
    }
}

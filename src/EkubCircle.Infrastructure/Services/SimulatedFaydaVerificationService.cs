using System.Security.Cryptography;
using System.Text;
using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EkubCircle.Infrastructure.Services;

public class SimulatedFaydaVerificationService : IFaydaVerificationService
{
    private readonly IApplicationDbContext _context;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<SimulatedFaydaVerificationService> _logger;

    public SimulatedFaydaVerificationService(
        IApplicationDbContext context,
        IHostEnvironment environment,
        ILogger<SimulatedFaydaVerificationService> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, string? SimulatedOtp)> RequestVerificationAsync(
        string userId,
        string fan,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fan))
        {
            return (false, "Fayda FAN is required.", null);
        }

        // Generate simulated 6-digit OTP code
        var otpCode = fan.StartsWith("DEMO", StringComparison.OrdinalIgnoreCase)
            ? "123456"
            : Random.Shared.Next(100000, 999999).ToString();

        var otpHash = HashOtp(otpCode);

        var otpEntity = new OtpVerification
        {
            UserId = userId,
            PhoneNumber = phoneNumber,
            OtpHash = otpHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            Attempts = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.OtpVerifications.Add(otpEntity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Simulated Fayda verification OTP generated for user {UserId}: {OtpCode}", userId, otpCode);

        // Only return simulated OTP in development for automated testing/demo UX
        string? devOtp = _environment.IsDevelopment() ? otpCode : null;

        return (true, "OTP generated and sent to user's registered phone.", devOtp);
    }

    public async Task<(bool Success, string Message)> VerifyOtpAsync(
        string userId,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return (false, "Verification code is required.");
        }

        var otpRecord = await _context.OtpVerifications
            .Where(o => o.UserId == userId && o.VerifiedAt == null)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otpRecord == null)
        {
            return (false, "No active verification request found. Please request a new verification code.");
        }

        if (otpRecord.IsExpired)
        {
            return (false, "Verification code has expired. Please request a new code.");
        }

        if (otpRecord.Attempts >= 5)
        {
            return (false, "Too many failed attempts. Please request a new verification code.");
        }

        var providedHash = HashOtp(code.Trim());
        if (providedHash != otpRecord.OtpHash)
        {
            otpRecord.Attempts++;
            await _context.SaveChangesAsync(cancellationToken);
            return (false, "Invalid verification code.");
        }

        otpRecord.VerifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return (true, "Fayda verification successfully completed.");
    }

    private static string HashOtp(string otp)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
        return Convert.ToHexString(bytes);
    }
}

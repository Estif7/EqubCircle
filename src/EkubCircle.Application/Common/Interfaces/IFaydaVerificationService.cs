namespace EkubCircle.Application.Common.Interfaces;

public interface IFaydaVerificationService
{
    Task<(bool Success, string Message, string? SimulatedOtp)> RequestVerificationAsync(
        string userId,
        string fan,
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> VerifyOtpAsync(
        string userId,
        string code,
        CancellationToken cancellationToken = default);
}

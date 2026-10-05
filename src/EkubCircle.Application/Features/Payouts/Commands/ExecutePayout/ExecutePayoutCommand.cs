using EkubCircle.Application.DTOs.Payouts;
using MediatR;

namespace EkubCircle.Application.Features.Payouts.Commands.ExecutePayout;

public record ExecutePayoutCommand(Guid RoundId) : IRequest<PayoutDto>;

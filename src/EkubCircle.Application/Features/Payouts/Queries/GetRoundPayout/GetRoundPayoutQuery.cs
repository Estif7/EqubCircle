using EkubCircle.Application.DTOs.Payouts;
using MediatR;

namespace EkubCircle.Application.Features.Payouts.Queries.GetRoundPayout;

public record GetRoundPayoutQuery(Guid RoundId) : IRequest<PayoutDto?>;

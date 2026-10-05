using EkubCircle.Application.DTOs.Payments;
using MediatR;

namespace EkubCircle.Application.Features.Payments.Queries.GetRoundPaymentProgress;

public record GetRoundPaymentProgressQuery(Guid RoundId) : IRequest<RoundPaymentProgressDto>;

using EkubCircle.Application.DTOs.Payments;
using MediatR;

namespace EkubCircle.Application.Features.Payments.Queries.GetRoundPayments;

public record GetRoundPaymentsQuery(Guid RoundId) : IRequest<List<PaymentDto>>;

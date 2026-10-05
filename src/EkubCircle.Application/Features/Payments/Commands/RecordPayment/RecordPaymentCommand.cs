using EkubCircle.Application.DTOs.Payments;
using MediatR;

namespace EkubCircle.Application.Features.Payments.Commands.RecordPayment;

public record RecordPaymentCommand(Guid RoundId, RecordPaymentRequest Request) : IRequest<PaymentDto>;

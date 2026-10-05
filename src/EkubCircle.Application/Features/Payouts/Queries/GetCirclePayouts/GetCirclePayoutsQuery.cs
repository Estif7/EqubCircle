using EkubCircle.Application.DTOs.Payouts;
using MediatR;

namespace EkubCircle.Application.Features.Payouts.Queries.GetCirclePayouts;

public record GetCirclePayoutsQuery(Guid CircleId) : IRequest<List<PayoutDto>>;

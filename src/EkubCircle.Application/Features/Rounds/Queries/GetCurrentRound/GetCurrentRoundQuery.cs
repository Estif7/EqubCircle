using EkubCircle.Application.DTOs.Rounds;
using MediatR;

namespace EkubCircle.Application.Features.Rounds.Queries.GetCurrentRound;

public record GetCurrentRoundQuery(Guid CircleId) : IRequest<RoundDto?>;

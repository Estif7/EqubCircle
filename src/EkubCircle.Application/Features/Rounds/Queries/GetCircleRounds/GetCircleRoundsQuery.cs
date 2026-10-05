using EkubCircle.Application.DTOs.Rounds;
using MediatR;

namespace EkubCircle.Application.Features.Rounds.Queries.GetCircleRounds;

public record GetCircleRoundsQuery(Guid CircleId) : IRequest<List<RoundDto>>;

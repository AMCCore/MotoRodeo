using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Кандидаты в судьи (учётки с правом CanJudge).
/// </summary>
/// <param name="ExcludeAccountIds">Учётки, которых не показывать (например уже участники).</param>
public sealed record GetJudgeCandidatesQuery(IReadOnlyList<Guid>? ExcludeAccountIds = null)
    : IRequest<IReadOnlyList<NamedAccountDto>>;
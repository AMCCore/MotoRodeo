using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Запрос кандидатов в судьи: учётные записи с правом судить заезды.
/// </summary>
/// <returns>Отсортированный по имени список кандидатов.</returns>
public sealed record GetJudgeCandidatesQuery() : IRequest<IReadOnlyList<NamedAccountDto>>;

using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Команда создания события. Участники не назначаются вручную: ими становятся зарегистрировавшиеся пользователи.
/// </summary>
/// <param name="Title">Название.</param>
/// <param name="Place">Место проведения.</param>
/// <param name="EventDate">Дата и время события.</param>
/// <param name="RegistrationClosesAt">Дата окончания регистрации; <c>null</c> — начало события минус значение по умолчанию из настроек.</param>
/// <param name="GroupCount">Количество групп для жеребьёвки.</param>
/// <param name="JudgeIds">Назначенные судьи.</param>
/// <returns>Идентификатор созданного события.</returns>
public sealed record CreateEventCommand(
    string Title,
    string Place,
    DateTimeOffset EventDate,
    DateTimeOffset? RegistrationClosesAt,
    int GroupCount,
    IReadOnlyList<Guid> JudgeIds) : IRequest<Guid>;

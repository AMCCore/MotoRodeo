using MediatR;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Запрос на восстановление пароля по электронной почте.
/// </summary>
/// <param name="Email">Электронная почта (логин).</param>
/// <param name="ResetLinkFactory">Фабрика абсолютной ссылки восстановления по Id записи запроса.</param>
public sealed record RequestPasswordResetCommand(
    string Email,
    Func<Guid, string> ResetLinkFactory) : IRequest;

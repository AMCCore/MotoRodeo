using MediatR;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Команда регистрации нового пользователя с подтверждением по электронной почте.
/// </summary>
/// <param name="FirstName">Имя.</param>
/// <param name="LastName">Фамилия.</param>
/// <param name="Nickname">Обращение/прозвище.</param>
/// <param name="Email">Электронная почта (логин).</param>
/// <param name="Password">Пароль в открытом виде.</param>
/// <param name="ConfirmationLinkFactory">Фабрика абсолютной ссылки подтверждения по Id учётной записи.</param>
public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string? Nickname,
    string Email,
    string Password,
    Func<Guid, string> ConfirmationLinkFactory) : IRequest;
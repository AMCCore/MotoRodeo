using MediatR;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Команда регистрации нового пользователя.
/// </summary>
/// <param name="Name">Отображаемое имя.</param>
/// <param name="Login">Логин для входа.</param>
/// <param name="Password">Пароль в открытом виде (будет захеширован).</param>
/// <returns>Идентификатор созданной учётной записи.</returns>
public sealed record RegisterUserCommand(string FirstName, string LastName, string? Login, string Password) : IRequest<Guid>;

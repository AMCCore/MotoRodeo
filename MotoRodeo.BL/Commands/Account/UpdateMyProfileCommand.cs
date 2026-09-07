using MediatR;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Обновление профиля текущего пользователя.
/// </summary>
/// <param name="FirstName">Имя.</param>
/// <param name="LastName">Фамилия.</param>
/// <param name="Nickname">Прозвище.</param>
/// <param name="Vehicle">Транспортное средство.</param>
public sealed record UpdateMyProfileCommand(string FirstName, string LastName, string? Nickname, string? Vehicle) : IRequest;
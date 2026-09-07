using MediatR;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Смена пароля текущего пользователя.
/// </summary>
/// <param name="CurrentPassword">Текущий пароль.</param>
/// <param name="NewPassword">Новый пароль.</param>
public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;
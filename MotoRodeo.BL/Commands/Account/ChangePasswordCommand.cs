using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Смена пароля текущего пользователя.
/// </summary>
/// <param name="Dto">Текущий и новый пароль.</param>
public sealed record ChangePasswordCommand(ChangePasswordDto Dto) : IRequest;

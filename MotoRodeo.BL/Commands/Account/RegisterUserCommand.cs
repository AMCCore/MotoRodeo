using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Команда регистрации нового пользователя с подтверждением по электронной почте.
/// </summary>
/// <param name="Dto">Данные регистрации.</param>
public sealed record RegisterUserCommand(RegisterUserDto Dto) : IRequest;

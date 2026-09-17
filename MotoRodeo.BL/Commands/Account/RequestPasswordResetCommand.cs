using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Запрос на восстановление пароля по электронной почте.
/// </summary>
/// <param name="Dto">Email и фабрика ссылки.</param>
public sealed record RequestPasswordResetCommand(RequestPasswordResetDto Dto) : IRequest;
